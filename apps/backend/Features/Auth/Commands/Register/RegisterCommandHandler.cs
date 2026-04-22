using backend.Features.Auth.Domain;
using backend.Features.Auth.Contracts;
using backend.Features.Auth.Persistence;
using backend.Features.Auth.Services;

namespace backend.Features.Auth.Commands.Register;

public sealed class RegisterCommandHandler(
    IAuthRepository repository,
    PasswordService passwordService,
    AuthSessionService authSessions)
{
    public async Task<ApplicationResult<AuthResponse>> Handle(RegisterRequest request, CancellationToken cancellationToken)
    {
        if (await repository.EmailExistsAsync(request.Email, cancellationToken))
        {
            return ApplicationResult<AuthResponse>.BadRequest("Registration failed", new Dictionary<string, string[]>
            {
                ["email"] = ["Email already exists"]
            });
        }

        var user = new User { FullName = request.FullName, Email = request.Email };
        user.PasswordHash = passwordService.HashPassword(user, request.Password);

        user = await repository.CreateUserAsync(user.FullName, user.Email, user.PasswordHash, cancellationToken);

        var response = await authSessions.CreateSessionAsync(user, cancellationToken);

        return ApplicationResult<AuthResponse>.Ok(response, "Registration successful");
    }
}
