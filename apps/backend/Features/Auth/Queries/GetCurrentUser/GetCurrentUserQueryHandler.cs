using backend.Features.Auth.Persistence;
using Microsoft.AspNetCore.Http;

namespace backend.Features.Auth.Queries.GetCurrentUser;

public sealed class GetCurrentUserQueryHandler(
    AuthRepository repository,
    IHttpContextAccessor httpContextAccessor)
{
    public async Task<ApplicationResult<CurrentUserResponse>> Handle(GetCurrentUserRequest request, CancellationToken cancellationToken)
    {
        var sessionClaim = httpContextAccessor.HttpContext?.User.FindFirst("sid")?.Value;
        if (!Guid.TryParse(sessionClaim, out var sessionPublicId))
        {
            return ApplicationResult<CurrentUserResponse>.Unauthorized("Authentication required", new Dictionary<string, string[]> { ["auth"] = ["A valid authenticated session is required"] });
        }

        var user = await repository.GetUserBySessionAsync(sessionPublicId, cancellationToken);
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
