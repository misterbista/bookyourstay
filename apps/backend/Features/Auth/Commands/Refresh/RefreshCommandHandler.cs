using backend.Features.Auth.Contracts;
using backend.Features.Auth.Services;
using Microsoft.AspNetCore.Http;

namespace backend.Features.Auth.Commands.Refresh;

public sealed class RefreshCommandHandler(
    AuthSessionService authSessions,
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

        var response = await authSessions.RotateSessionAsync(refreshToken, cancellationToken);
        if (response is null)
        {
            return ApplicationResult<AuthResponse>.Unauthorized("Token refresh failed", new Dictionary<string, string[]>
            {
                ["token"] = ["Invalid or expired refresh token"]
            });
        }

        return ApplicationResult<AuthResponse>.Ok(response, "Token refreshed");
    }
}
