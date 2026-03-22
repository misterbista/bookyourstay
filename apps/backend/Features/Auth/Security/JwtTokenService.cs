using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend.Features.Auth.Domain;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace backend.Features.Auth.Security;

public sealed class JwtTokenService
{
    private readonly JwtOptions _options;
    private readonly TimeProvider _timeProvider;
    private readonly JwtSecurityTokenHandler _tokenHandler = new();
    private readonly SigningCredentials _signingCredentials;
    private readonly TokenValidationParameters _validationParameters;

    public JwtTokenService(IOptions<JwtOptions> options, TimeProvider timeProvider)
    {
        _options = options.Value;
        _timeProvider = timeProvider;

        var secretKeyBytes = Encoding.UTF8.GetBytes(_options.SecretKey);
        var signingKey = new SymmetricSecurityKey(secretKeyBytes);

        _signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        _validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ValidateIssuer = true,
            ValidIssuer = _options.Issuer,
            ValidateAudience = true,
            ValidAudience = _options.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    }

    public (string Token, DateTimeOffset ExpiresAt) CreateAccessToken(AuthUser user, AuthSession session)
    {
        var now = _timeProvider.GetUtcNow();
        var expiresAt = now.AddMinutes(_options.AccessTokenLifetimeMinutes);
        Claim[] claims =
        {
            new(JwtRegisteredClaimNames.Sub, user.PublicId.ToString()),
            new("uid", user.Id.ToString()),
            new("sid", session.PublicId.ToString())
        };

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            Subject = new ClaimsIdentity(claims),
            Expires = expiresAt.UtcDateTime,
            SigningCredentials = _signingCredentials
        };

        var token = _tokenHandler.CreateToken(descriptor);
        return (_tokenHandler.WriteToken(token), expiresAt);
    }

    public bool TryValidateAccessToken(string token, out JwtAccessTokenPayload payload)
    {
        payload = default!;

        try
        {
            var principal = _tokenHandler.ValidateToken(token, _validationParameters, out _);

            var userIdClaim = principal.FindFirst("uid")?.Value;
            var sessionIdClaim = principal.FindFirst("sid")?.Value;
            var subjectClaim = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (!long.TryParse(userIdClaim, out var userId) ||
                !Guid.TryParse(sessionIdClaim, out var sessionPublicId) ||
                !Guid.TryParse(subjectClaim, out var userPublicId))
            {
                return false;
            }

            payload = new JwtAccessTokenPayload(userId, userPublicId, sessionPublicId);
            return true;
        }
        catch (SecurityTokenException)
        {
            return false;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }
}
