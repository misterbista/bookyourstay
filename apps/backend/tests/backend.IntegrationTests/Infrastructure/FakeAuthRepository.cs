using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using backend.Features.Auth.Domain;
using backend.Features.Auth.Persistence;

namespace backend.IntegrationTests.Infrastructure;

public sealed class FakeAuthRepository : IAuthRepository
{
    private readonly object _gate = new();
    private readonly Dictionary<string, User> _usersByEmail = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<Guid, SessionRecord> _sessionsByPublicId = new();
    private readonly Dictionary<string, Guid> _sessionIdsByRefreshTokenHash = new(StringComparer.Ordinal);
    private long _nextUserId = 1;
    private long _nextSessionId = 1;

    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken)
    {
        lock (_gate)
        {
            return Task.FromResult(_usersByEmail.ContainsKey(email));
        }
    }

    public Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        lock (_gate)
        {
            return Task.FromResult(_usersByEmail.GetValueOrDefault(email)?.Clone());
        }
    }

    public Task<User> CreateUserAsync(string fullName, string email, string passwordHash, CancellationToken cancellationToken)
    {
        lock (_gate)
        {
            if (_usersByEmail.ContainsKey(email))
            {
                throw new InvalidOperationException("Email already exists.");
            }

            var user = new User
            {
                Id = _nextUserId++,
                PublicId = Guid.NewGuid(),
                FullName = fullName,
                Email = email,
                PasswordHash = passwordHash,
                Status = "active",
                EmailVerifiedAt = DateTimeOffset.UtcNow,
                CreatedAt = DateTimeOffset.UtcNow
            };

            _usersByEmail[email] = user;
            return Task.FromResult(user.Clone());
        }
    }

    public Task<Session> CreateSessionAsync(long userId, string refreshTokenHash, DateTimeOffset expiresAt, CancellationToken cancellationToken)
    {
        lock (_gate)
        {
            var session = new Session
            {
                Id = _nextSessionId++,
                PublicId = Guid.NewGuid(),
                UserId = userId,
                RefreshTokenHash = refreshTokenHash,
                ExpiresAt = expiresAt,
                CreatedAt = DateTimeOffset.UtcNow
            };

            _sessionsByPublicId[session.PublicId] = new SessionRecord(session);
            _sessionIdsByRefreshTokenHash[refreshTokenHash] = session.PublicId;
            return Task.FromResult(session.Clone());
        }
    }

    public Task<(User User, Session Session)?> RotateSessionAsync(string oldRefreshTokenHash, string newRefreshTokenHash, DateTimeOffset newExpiresAt, CancellationToken cancellationToken)
    {
        lock (_gate)
        {
            if (!_sessionIdsByRefreshTokenHash.TryGetValue(oldRefreshTokenHash, out var sessionPublicId) ||
                !_sessionsByPublicId.TryGetValue(sessionPublicId, out var sessionRecord) ||
                sessionRecord.RevokedAt is not null ||
                sessionRecord.Session.ExpiresAt <= DateTimeOffset.UtcNow)
            {
                return Task.FromResult<(User, Session)?>(null);
            }

            _sessionIdsByRefreshTokenHash.Remove(oldRefreshTokenHash);
            sessionRecord.Session.RefreshTokenHash = newRefreshTokenHash;
            sessionRecord.Session.ExpiresAt = newExpiresAt;
            _sessionIdsByRefreshTokenHash[newRefreshTokenHash] = sessionRecord.Session.PublicId;

            var user = _usersByEmail.Values.SingleOrDefault(candidate => candidate.Id == sessionRecord.Session.UserId);
            if (user is null)
            {
                return Task.FromResult<(User, Session)?>(null);
            }

            user.LastLoginAt = DateTimeOffset.UtcNow;
            return Task.FromResult<(User, Session)?>((user.Clone(), sessionRecord.Session.Clone()));
        }
    }

    public Task<User?> GetUserBySessionAsync(Guid sessionPublicId, CancellationToken cancellationToken)
    {
        lock (_gate)
        {
            if (!_sessionsByPublicId.TryGetValue(sessionPublicId, out var sessionRecord) ||
                sessionRecord.RevokedAt is not null ||
                sessionRecord.Session.ExpiresAt <= DateTimeOffset.UtcNow)
            {
                return Task.FromResult<User?>(null);
            }

            var user = _usersByEmail.Values.SingleOrDefault(candidate => candidate.Id == sessionRecord.Session.UserId);
            return Task.FromResult(user?.Clone());
        }
    }

    public Task<bool> RevokeSessionAsync(Guid sessionPublicId, CancellationToken cancellationToken)
    {
        lock (_gate)
        {
            if (!_sessionsByPublicId.TryGetValue(sessionPublicId, out var sessionRecord) ||
                sessionRecord.RevokedAt is not null)
            {
                return Task.FromResult(false);
            }

            sessionRecord.RevokedAt = DateTimeOffset.UtcNow;
            sessionRecord.Session.ExpiresAt = DateTimeOffset.UtcNow;
            return Task.FromResult(true);
        }
    }

    private sealed class SessionRecord(Session session)
    {
        public Session Session { get; } = session;
        public DateTimeOffset? RevokedAt { get; set; }
    }
}

internal static class AuthRepositoryTestCloning
{
    public static User Clone(this User user) =>
        new()
        {
            Id = user.Id,
            PublicId = user.PublicId,
            FullName = user.FullName,
            Email = user.Email,
            PasswordHash = user.PasswordHash,
            Status = user.Status,
            EmailVerifiedAt = user.EmailVerifiedAt,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt
        };

    public static Session Clone(this Session session) =>
        new()
        {
            Id = session.Id,
            PublicId = session.PublicId,
            UserId = session.UserId,
            RefreshTokenHash = session.RefreshTokenHash,
            ExpiresAt = session.ExpiresAt,
            CreatedAt = session.CreatedAt
        };
}
