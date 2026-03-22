using FluentValidation;

namespace backend.Features.Auth.Commands.Register;

public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("Full name is required.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("A valid email is required.")
            .EmailAddress()
            .WithMessage("A valid email is required.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password must be at least 8 characters long.")
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long.");
    }
}
