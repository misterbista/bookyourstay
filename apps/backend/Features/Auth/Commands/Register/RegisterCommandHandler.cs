using backend.Features.Auth.Domain;
using backend.Features.Auth.Contracts;
using backend.Features.Auth.Persistence;
using backend.Features.Auth.Services;

namespace backend.Features.Auth.Commands.Register;

public sealed class RegisterCommandHandler(
    AuthRepository repository,
    PasswordService passwordService,
    JwtService jwtService,
    TimeProvider timeProvider)
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

        var refreshToken = PasswordService.GenerateRefreshToken();
        var refreshTokenHash = PasswordService.HashToken(refreshToken);
        var sessionExpiresAt = timeProvider.GetUtcNow().AddDays(7);

        var session = await repository.CreateSessionAsync(user.Id, refreshTokenHash, sessionExpiresAt, cancellationToken);

        var accessToken = jwtService.CreateAccessToken(user, session);
        var response = new AuthResponse(accessToken.Token, accessToken.ExpiresAt, refreshToken, session.ExpiresAt, session.PublicId);

        return ApplicationResult<AuthResponse>.Ok(response, "Registration successful");
    }
}
