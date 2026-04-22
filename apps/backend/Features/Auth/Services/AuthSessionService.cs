using backend.Features.Auth.Contracts;
using backend.Features.Auth.Domain;
using backend.Features.Auth.Options;
using backend.Features.Auth.Persistence;
using Microsoft.Extensions.Options;

namespace backend.Features.Auth.Services;

public sealed class AuthSessionService(
    IAuthRepository repository,
    JwtService jwtService,
    TimeProvider timeProvider,
    IOptions<JwtOptions> jwtOptions)
{
    public async Task<AuthResponse> CreateSessionAsync(User user, CancellationToken cancellationToken)
    {
        var refreshToken = PasswordService.GenerateRefreshToken();
        var refreshTokenHash = PasswordService.HashToken(refreshToken);
        var expiresAt = GetRefreshTokenExpiry();

        var session = await repository.CreateSessionAsync(user.Id, refreshTokenHash, expiresAt, cancellationToken);
        return CreateAuthResponse(user, session, refreshToken);
    }

    public async Task<AuthResponse?> RotateSessionAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var oldTokenHash = PasswordService.HashToken(refreshToken);
        var newRefreshToken = PasswordService.GenerateRefreshToken();
        var newTokenHash = PasswordService.HashToken(newRefreshToken);
        var expiresAt = GetRefreshTokenExpiry();

        var result = await repository.RotateSessionAsync(oldTokenHash, newTokenHash, expiresAt, cancellationToken);
        if (result is null)
        {
            return null;
        }

        var (user, session) = result.Value;
        return CreateAuthResponse(user, session, newRefreshToken);
    }

    private DateTimeOffset GetRefreshTokenExpiry() =>
        timeProvider.GetUtcNow().AddDays(jwtOptions.Value.RefreshTokenLifetimeDays);

    private AuthResponse CreateAuthResponse(User user, Session session, string refreshToken)
    {
        var accessToken = jwtService.CreateAccessToken(user, session);
        return new AuthResponse(
            accessToken.Token,
            accessToken.ExpiresAt,
            refreshToken,
            session.ExpiresAt,
            session.PublicId);
    }
}
