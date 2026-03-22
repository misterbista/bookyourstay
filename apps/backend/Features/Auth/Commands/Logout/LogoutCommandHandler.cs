using backend.Features.Auth.Persistence;
using backend.Features.Auth.Security;
using EzyMediatr.Core.Handlers;

namespace backend.Features.Auth.Commands.Logout;

public sealed class LogoutCommandHandler(AuthRepository repository, JwtTokenService jwtTokenService, TimeProvider timeProvider)
    : IRequestHandler<LogoutRequest, ApplicationResult>
{
    public async Task<ApplicationResult> Handle(LogoutRequest request, CancellationToken cancellationToken)
    {
        if (!jwtTokenService.TryValidateAccessToken(request.Token, out var payload))
        {
            return ApplicationResult.Unauthorized("Logout failed", new Dictionary<string, string[]>
            {
                ["authorization"] = ["The provided token is invalid or expired."]
            });
        }

        var revoked = await repository.RevokeSessionAsync(payload.SessionPublicId, "user_logout", timeProvider.GetUtcNow(), cancellationToken);

        return revoked
            ? ApplicationResult.Ok("Logout successful")
            : ApplicationResult.NotFound("Session not found", new Dictionary<string, string[]>
            {
                ["session"] = ["The provided session token is invalid or already expired."]
            });
    }
}
