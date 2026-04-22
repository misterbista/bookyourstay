using backend.Features.Auth.Domain;
using backend.Features.Auth.Contracts;
using backend.Features.Auth.Persistence;
using backend.Features.Auth.Services;
using Microsoft.AspNetCore.Identity;

namespace backend.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler(
    IAuthRepository repository,
    PasswordService passwordService,
    AuthSessionService authSessions)
{
    public async Task<ApplicationResult<AuthResponse>> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await repository.GetUserByEmailAsync(request.Email, cancellationToken);
        if (user is null)
        {
            return InvalidCredentials();
        }

        if (passwordService.VerifyPassword(user, request.Password) != PasswordVerificationResult.Success)
        {
            return InvalidCredentials();
        }

        var response = await authSessions.CreateSessionAsync(user, cancellationToken);

        return ApplicationResult<AuthResponse>.Ok(response, "Login successful");
    }

    private static ApplicationResult<AuthResponse> InvalidCredentials() =>
        ApplicationResult<AuthResponse>.Unauthorized("Login failed", new Dictionary<string, string[]>
        {
            ["credentials"] = ["Invalid email or password"]
        });
}
