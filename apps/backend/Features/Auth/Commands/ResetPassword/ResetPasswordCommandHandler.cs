using backend.Features.Auth.Domain;
using backend.Features.Auth.Persistence;
using EzyMediatr.Core.Handlers;
using Microsoft.AspNetCore.Identity;

namespace backend.Features.Auth.Commands.ResetPassword;

public sealed class ResetPasswordCommandHandler(
    AuthRepository repository,
    IPasswordHasher<AuthIdentity> passwordHasher,
    TimeProvider timeProvider)
    : IRequestHandler<ResetPasswordRequest, ApplicationResult>
{
    public async Task<ApplicationResult> Handle(ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        var tokenHash = Security.TokenHasher.Hash(request.ResetToken);
        var resetContext = await repository.GetPasswordResetContextAsync(tokenHash, cancellationToken);
        if (resetContext is null)
        {
            return InvalidToken("Reset token is invalid.");
        }

        if (resetContext.Ticket.ExpiresAt <= now)
        {
            return InvalidToken("Reset token has expired.");
        }

        if (resetContext.Ticket.ConsumedAt is not null)
        {
            return InvalidToken("Reset token has already been used.");
        }

        if (resetContext.User.DeletedAt is not null)
        {
            return ApplicationResult.NotFound("Reset password failed", new Dictionary<string, string[]>
            {
                ["user"] = ["The user for this reset token no longer exists."]
            });
        }

        var identity = resetContext.Identity;
        identity.PasswordHash = passwordHasher.HashPassword(identity, request.NewPassword);
        var updated = await repository.UpdatePasswordAndConsumeResetTokenAsync(
            resetContext.User.Id,
            identity.Id,
            identity.PasswordHash,
            tokenHash,
            now,
            cancellationToken);

        if (!updated)
        {
            return InvalidToken("Reset token has already been used.");
        }

        return ApplicationResult.Ok("Password reset successful");
    }

    private static ApplicationResult InvalidToken(string message) =>
        ApplicationResult.BadRequest("Reset password failed", new Dictionary<string, string[]>
        {
            ["resetToken"] = [message]
        });
}
