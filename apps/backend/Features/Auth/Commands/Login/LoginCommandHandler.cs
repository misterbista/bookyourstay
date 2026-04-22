using backend.Features.Auth.Domain;
using backend.Features.Auth.Contracts;
using backend.Features.Auth.Persistence;
using backend.Features.Auth.Services;
using Microsoft.AspNetCore.Identity;

namespace backend.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler(
    AuthRepository repository,
    PasswordService passwordService,
    JwtService jwtService,
    TimeProvider timeProvider)
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

        var refreshToken = PasswordService.GenerateRefreshToken();
        var refreshTokenHash = PasswordService.HashToken(refreshToken);
        var sessionExpiresAt = timeProvider.GetUtcNow().AddDays(7);

        var session = await repository.CreateSessionAsync(user.Id, refreshTokenHash, sessionExpiresAt, cancellationToken);

        var accessToken = jwtService.CreateAccessToken(user, session);
        var response = new AuthResponse(accessToken.Token, accessToken.ExpiresAt, refreshToken, session.ExpiresAt, session.PublicId);

        return ApplicationResult<AuthResponse>.Ok(response, "Login successful");
    }

    private static ApplicationResult<AuthResponse> InvalidCredentials() =>
        ApplicationResult<AuthResponse>.Unauthorized("Login failed", new Dictionary<string, string[]>
        {
            ["credentials"] = ["Invalid email or password"]
        });
}
