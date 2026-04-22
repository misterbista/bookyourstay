using backend.Features.Auth.Contracts;
using backend.Features.Auth.Persistence;
using backend.Features.Auth.Services;
using Microsoft.AspNetCore.Http;

namespace backend.Features.Auth.Commands.Refresh;

public sealed class RefreshCommandHandler(
    AuthRepository repository,
    JwtService jwtService,
    TimeProvider timeProvider,
    AuthCookieService authCookies,
    IHttpContextAccessor httpContextAccessor)
{
    public async Task<ApplicationResult<AuthResponse>> Handle(RefreshRequest request, CancellationToken cancellationToken)
    {
        var httpContext = httpContextAccessor.HttpContext;
        var refreshToken = httpContext is null ? null : authCookies.GetRefreshToken(httpContext);
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return ApplicationResult<AuthResponse>.Unauthorized("No refresh token", new Dictionary<string, string[]>
            {
                ["token"] = ["Refresh token not found in cookies"]
            });
        }

        var oldTokenHash = PasswordService.HashToken(refreshToken);
        var newRefreshToken = PasswordService.GenerateRefreshToken();
        var newTokenHash = PasswordService.HashToken(newRefreshToken);
        var newExpiresAt = timeProvider.GetUtcNow().AddDays(7);

        var result = await repository.RotateSessionAsync(oldTokenHash, newTokenHash, newExpiresAt, cancellationToken);
        if (result is null)
        {
            return ApplicationResult<AuthResponse>.Unauthorized("Token refresh failed", new Dictionary<string, string[]>
            {
                ["token"] = ["Invalid or expired refresh token"]
            });
        }

        var (user, session) = result.Value;
        var accessToken = jwtService.CreateAccessToken(user, session);
        var response = new AuthResponse(accessToken.Token, accessToken.ExpiresAt, newRefreshToken, session.ExpiresAt, session.PublicId);

        return ApplicationResult<AuthResponse>.Ok(response, "Token refreshed");
    }
}
