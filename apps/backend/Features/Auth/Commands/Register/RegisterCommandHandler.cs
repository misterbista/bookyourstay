using backend.Features.Auth.Domain;
using backend.Features.Auth.Persistence;
using backend.Features.Auth.Security;
using EzyMediatr.Core.Handlers;
using Microsoft.AspNetCore.Identity;

namespace backend.Features.Auth.Commands.Register;

public sealed class RegisterCommandHandler(
    AuthRepository repository,
    IPasswordHasher<AuthIdentity> passwordHasher,
    JwtTokenService jwtTokenService,
    TimeProvider timeProvider)
    : IRequestHandler<RegisterRequest, ApplicationResult<AuthResponse>>
{
    public async Task<ApplicationResult<AuthResponse>> Handle(RegisterRequest request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        if (await repository.EmailExistsAsync(normalizedEmail, cancellationToken))
        {
            return ApplicationResult<AuthResponse>.BadRequest("Registration failed", new Dictionary<string, string[]>
            {
                ["email"] = ["An account with this email already exists."]
            });
        }

        var identity = new AuthIdentity { UserId = 0 };
        identity.PasswordHash = passwordHasher.HashPassword(identity, request.Password);

        var refreshToken = AuthTokenFactory.CreateRefreshToken();
        var sessionExpiresAt = timeProvider.GetUtcNow().Add(AuthDefaults.SessionLifetime);
        var registered = await repository.RegisterLocalUserAsync(
            request.FullName.Trim(),
            normalizedEmail,
            identity.PasswordHash,
            AuthUserStatuses.PendingVerification,
            TokenHasher.Hash(refreshToken),
            sessionExpiresAt,
            cancellationToken);

        var accessToken = jwtTokenService.CreateAccessToken(registered.User, registered.Session);
        var response = new AuthResponse(
            registered.User.PublicId,
            registered.User.FullName,
            registered.User.Email ?? string.Empty,
            accessToken.Token,
            accessToken.ExpiresAt,
            refreshToken,
            registered.Session.ExpiresAt,
            registered.Session.PublicId,
            registered.User.Status,
            registered.User.EmailVerifiedAt);

        return ApplicationResult<AuthResponse>.Ok(response, "Registration successful");
    }
}
