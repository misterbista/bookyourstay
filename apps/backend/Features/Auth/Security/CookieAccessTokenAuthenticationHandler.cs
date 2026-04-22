using System.Security.Claims;
using System.Text.Encodings.Web;
using backend.Features.Auth.Persistence;
using backend.Features.Auth.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace backend.Features.Auth.Security;

public sealed class CookieAccessTokenAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    AuthCookieService authCookies,
    JwtService jwtService,
    IAuthRepository repository)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var accessToken = authCookies.GetAccessToken(Context);
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return AuthenticateResult.NoResult();
        }

        if (!jwtService.TryValidateAccessToken(accessToken, out var payload))
        {
            return AuthenticateResult.Fail("Invalid access token.");
        }

        var user = await repository.GetUserBySessionAsync(payload.SessionPublicId, Context.RequestAborted);
        if (user is null)
        {
            return AuthenticateResult.Fail("Session is not active.");
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.PublicId.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(AuthClaims.UserId, user.Id.ToString()),
            new Claim(AuthClaims.SessionId, payload.SessionPublicId.ToString())
        };

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}
