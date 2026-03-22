using FluentValidation;

namespace backend.Features.Auth.Commands.ResetPassword;

public sealed class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.ResetToken)
            .NotEmpty()
            .WithMessage("Reset token is required.");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("New password must be at least 8 characters long.")
            .MinimumLength(8)
            .WithMessage("New password must be at least 8 characters long.");
    }
}
