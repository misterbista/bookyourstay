using backend.Features.Auth.Contracts;
using backend.Features.Auth.Domain;
using backend.Features.Auth.Persistence;
using Microsoft.AspNetCore.Identity;

namespace backend.Features.Auth.Services;

public sealed class AuthService(
    IAuthRepository repository,
    PasswordService passwords,
    AuthSessionService sessions,
    JwtService jwt)
{
    public async Task<ApplicationResult<AuthResponse>> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken)
    {
        if (await repository.EmailExistsAsync(request.Email, cancellationToken))
        {
            return ApplicationResult<AuthResponse>.BadRequest("Registration failed", new Dictionary<string, string[]>
            {
                ["email"] = ["Email already exists"]
            });
        }

        var user = new User { FullName = request.FullName, Email = request.Email };
        var passwordHash = passwords.HashPassword(user, request.Password);
        var savedUser = await repository.CreateUserAsync(user.FullName, user.Email, passwordHash, cancellationToken);
        var response = await sessions.CreateSessionAsync(savedUser, cancellationToken);

        return ApplicationResult<AuthResponse>.Ok(response, "Registration successful");
    }

    public async Task<ApplicationResult<AuthResponse>> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken)
    {
        var user = await repository.GetUserByEmailAsync(request.Email, cancellationToken);
        if (user is null ||
            passwords.VerifyPassword(user, request.Password) != PasswordVerificationResult.Success)
        {
            return InvalidCredentials();
        }

        var response = await sessions.CreateSessionAsync(user, cancellationToken);
        return ApplicationResult<AuthResponse>.Ok(response, "Login successful");
    }

    public async Task<ApplicationResult<AuthResponse>> RefreshAsync(
        string? refreshToken,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return ApplicationResult<AuthResponse>.Unauthorized("No refresh token", new Dictionary<string, string[]>
            {
                ["token"] = ["Refresh token not found in cookies"]
            });
        }

        var response = await sessions.RotateSessionAsync(refreshToken, cancellationToken);
        if (response is null)
        {
            return ApplicationResult<AuthResponse>.Unauthorized("Token refresh failed", new Dictionary<string, string[]>
            {
                ["token"] = ["Invalid or expired refresh token"]
            });
        }

        return ApplicationResult<AuthResponse>.Ok(response, "Token refreshed");
    }

    public async Task<ApplicationResult> LogoutAsync(
        string? accessToken,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(accessToken) &&
            jwt.TryValidateAccessToken(accessToken, out var payload))
        {
            await repository.RevokeSessionAsync(payload.SessionPublicId, cancellationToken);
        }

        return ApplicationResult.Ok("Logged out successfully");
    }

    public async Task<ApplicationResult<CurrentUserResponse>> GetCurrentUserAsync(
        Guid? sessionPublicId,
        CancellationToken cancellationToken)
    {
        if (sessionPublicId is null)
        {
            return ApplicationResult<CurrentUserResponse>.Unauthorized("Authentication required", new Dictionary<string, string[]>
            {
                ["auth"] = ["A valid authenticated session is required"]
            });
        }

        var user = await repository.GetUserBySessionAsync(sessionPublicId.Value, cancellationToken);
        if (user is null)
        {
            return ApplicationResult<CurrentUserResponse>.Unauthorized("Session expired", new Dictionary<string, string[]>
            {
                ["session"] = ["Session has expired"]
            });
        }

        var response = new CurrentUserResponse(
            user.PublicId,
            user.FullName,
            user.Email,
            user.Status,
            user.EmailVerifiedAt,
            user.CreatedAt,
            user.LastLoginAt);

        return ApplicationResult<CurrentUserResponse>.Ok(response, "Current user retrieved");
    }

    private static ApplicationResult<AuthResponse> InvalidCredentials() =>
        ApplicationResult<AuthResponse>.Unauthorized("Login failed", new Dictionary<string, string[]>
        {
            ["credentials"] = ["Invalid email or password"]
        });
}

public sealed record CurrentUserResponse(
    Guid Id,
    string FullName,
    string Email,
    string Status,
    DateTimeOffset? EmailVerifiedAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset? LastLoginAt);
