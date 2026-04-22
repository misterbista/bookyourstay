using backend.Features.Auth.Persistence;
using backend.Features.Auth.Services;
using Microsoft.AspNetCore.Http;

namespace backend.Features.Auth.Commands.Logout;

public sealed class LogoutCommandHandler(
    IAuthRepository repository,
    JwtService jwtService,
    AuthCookieService authCookies,
    IHttpContextAccessor httpContextAccessor)
{
    public async Task<ApplicationResult> Handle(LogoutRequest request, CancellationToken cancellationToken)
    {
        var httpContext = httpContextAccessor.HttpContext;
        var accessToken = httpContext is null ? null : authCookies.GetAccessToken(httpContext);
        if (!string.IsNullOrWhiteSpace(accessToken) &&
            jwtService.TryValidateAccessToken(accessToken, out var payload))
        {
            await repository.RevokeSessionAsync(payload.SessionPublicId, cancellationToken);
        }

        return ApplicationResult.Ok("Logged out successfully");
    }
}
