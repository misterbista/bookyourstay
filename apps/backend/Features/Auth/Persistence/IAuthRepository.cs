using backend.Features.Auth.Domain;

namespace backend.Features.Auth.Persistence;

public interface IAuthRepository
{
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken);
    Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken);
    Task<User> CreateUserAsync(string fullName, string email, string passwordHash, CancellationToken cancellationToken);
    Task<Session> CreateSessionAsync(long userId, string refreshTokenHash, DateTimeOffset expiresAt, CancellationToken cancellationToken);
    Task<(User User, Session Session)?> RotateSessionAsync(string oldRefreshTokenHash, string newRefreshTokenHash, DateTimeOffset newExpiresAt, CancellationToken cancellationToken);
    Task<User?> GetUserBySessionAsync(Guid sessionPublicId, CancellationToken cancellationToken);
    Task<bool> RevokeSessionAsync(Guid sessionPublicId, CancellationToken cancellationToken);
}
