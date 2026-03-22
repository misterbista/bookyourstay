using backend.Features.Auth.Persistence;
using backend.Features.Auth.Security;
using EzyMediatr.Core.Handlers;

namespace backend.Features.Auth.Queries.GetCurrentUser;

public sealed class GetCurrentUserQueryHandler(AuthRepository repository, JwtTokenService jwtTokenService, TimeProvider timeProvider)
    : IRequestHandler<GetCurrentUserRequest, ApplicationResult<CurrentUserResponse>>
{
    public async Task<ApplicationResult<CurrentUserResponse>> Handle(GetCurrentUserRequest request, CancellationToken cancellationToken)
    {
        if (!jwtTokenService.TryValidateAccessToken(request.Token, out var payload))
        {
            return Unauthorized();
        }

        var user = await repository.GetActiveUserBySessionAsync(payload.SessionPublicId, timeProvider.GetUtcNow(), cancellationToken);
        if (user is null || user.Id != payload.UserId || user.PublicId != payload.UserPublicId)
        {
            return Unauthorized();
        }

        return ApplicationResult<CurrentUserResponse>.Ok(
            new CurrentUserResponse(
                user.PublicId,
                user.FullName,
                user.Email ?? string.Empty,
                user.Status,
                user.EmailVerifiedAt,
                user.CreatedAt,
                user.LastLoginAt),
            "Current user fetched");
    }

    private static ApplicationResult<CurrentUserResponse> Unauthorized() =>
        ApplicationResult<CurrentUserResponse>.Unauthorized("Unauthorized", new Dictionary<string, string[]>
        {
            ["authorization"] = ["The provided token is invalid or expired."]
        });
}
