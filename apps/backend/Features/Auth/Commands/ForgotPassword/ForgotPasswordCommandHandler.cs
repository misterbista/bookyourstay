using backend.Features.Auth.Persistence;
using backend.Features.Auth.Security;
using EzyMediatr.Core.Handlers;

namespace backend.Features.Auth.Commands.ForgotPassword;

public sealed class ForgotPasswordCommandHandler(
    AuthRepository repository,
    TimeProvider timeProvider)
    : IRequestHandler<ForgotPasswordRequest, ApplicationResult<ForgotPasswordResponse>>
{
    public async Task<ApplicationResult<ForgotPasswordResponse>> Handle(ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var authRecord = await repository.GetLocalIdentityByEmailAsync(normalizedEmail, cancellationToken);
        if (authRecord is null)
        {
            return ApplicationResult<ForgotPasswordResponse>.NotFound("Password reset failed", new Dictionary<string, string[]>
            {
                ["email"] = ["No account was found for this email."]
            });
        }

        var token = AuthTokenFactory.CreatePasswordResetToken();
        var tokenHash = TokenHasher.Hash(token);
        var expiresAt = timeProvider.GetUtcNow().Add(AuthDefaults.PasswordResetTokenLifetime);

        await repository.StorePasswordResetTokenAsync(authRecord.Value.User.Id, tokenHash, expiresAt, cancellationToken);

        return ApplicationResult<ForgotPasswordResponse>.Ok(
            new ForgotPasswordResponse(token, expiresAt),
            "Password reset token generated",
            new { note = "For development clarity, the reset token is returned directly." });
    }
}
