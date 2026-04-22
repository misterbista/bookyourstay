using backend.Features.Auth.Persistence;
using backend.Features.Auth.Services;
using Microsoft.AspNetCore.Http;

namespace backend.Features.Auth.Queries.GetCurrentUser;

public sealed class GetCurrentUserQueryHandler(
    AuthRepository repository,
    JwtService jwtService,
    AuthCookieService authCookies,
    IHttpContextAccessor httpContextAccessor)
{
    public async Task<ApplicationResult<CurrentUserResponse>> Handle(GetCurrentUserRequest request, CancellationToken cancellationToken)
    {
        var httpContext = httpContextAccessor.HttpContext;
        var accessToken = httpContext is null ? null : authCookies.GetAccessToken(httpContext);
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return ApplicationResult<CurrentUserResponse>.Unauthorized("No access token", new Dictionary<string, string[]> { ["token"] = ["Access token not found in cookies"] });
        }

        if (!jwtService.TryValidateAccessToken(accessToken, out var payload))
        {
            return ApplicationResult<CurrentUserResponse>.Unauthorized("Invalid token", new Dictionary<string, string[]> { ["token"] = ["Invalid access token"] });
        }

        var user = await repository.GetUserBySessionAsync(payload.SessionPublicId, cancellationToken);
        if (user is null)
        {
            return ApplicationResult<CurrentUserResponse>.Unauthorized("Session expired", new Dictionary<string, string[]> { ["session"] = ["Session has expired"] });
        }

        var response = new CurrentUserResponse(
            user.PublicId,
            user.FullName,
            user.Email,
            user.Status,
            user.EmailVerifiedAt,
            user.CreatedAt,
            user.LastLoginAt);

        return ApplicationResult<CurrentUserResponse>.Ok(response, "Current user retrieved");
    }
}
