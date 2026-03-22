using backend.Features.Auth.Domain;
using backend.Features.Auth.Persistence;
using backend.Features.Auth.Security;
using EzyMediatr.Core.Handlers;
using Microsoft.AspNetCore.Identity;

namespace backend.Features.Auth.Commands.Login;

public sealed class LoginCommandHandler(
    AuthRepository repository,
    IPasswordHasher<AuthIdentity> passwordHasher,
    JwtTokenService jwtTokenService,
    TimeProvider timeProvider)
    : IRequestHandler<LoginRequest, ApplicationResult<AuthResponse>>
{
    public async Task<ApplicationResult<AuthResponse>> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var authRecord = await repository.GetLocalIdentityByEmailAsync(normalizedEmail, cancellationToken);
        if (authRecord is null)
        {
            return InvalidCredentials();
        }

        var user = authRecord.Value.User;
        var identity = authRecord.Value.Identity;

        if (string.IsNullOrWhiteSpace(identity.PasswordHash))
        {
            return InvalidCredentials();
        }

        var verification = passwordHasher.VerifyHashedPassword(identity, identity.PasswordHash, request.Password);
        if (verification == PasswordVerificationResult.Failed)
        {
            return InvalidCredentials();
        }

        var now = timeProvider.GetUtcNow();
        var refreshToken = AuthTokenFactory.CreateRefreshToken();
        var session = await repository.CreateSessionAsync(
            user.Id,
            identity.Id,
            TokenHasher.Hash(refreshToken),
            now.Add(AuthDefaults.SessionLifetime),
            now,
            cancellationToken);

        var accessToken = jwtTokenService.CreateAccessToken(user, session);
        var response = new AuthResponse(
            user.PublicId,
            user.FullName,
            user.Email ?? string.Empty,
            accessToken.Token,
            accessToken.ExpiresAt,
            refreshToken,
            session.ExpiresAt,
            session.PublicId,
            user.Status,
            user.EmailVerifiedAt);

        return ApplicationResult<AuthResponse>.Ok(response, "Login successful");
    }
    private static ApplicationResult<AuthResponse> InvalidCredentials() =>
        ApplicationResult<AuthResponse>.Unauthorized("Login failed", new Dictionary<string, string[]>
        {
            ["credentials"] = ["Email or password is incorrect."]
        });
}
