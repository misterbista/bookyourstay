using Dapper;
using backend.Features.Auth.Domain;
using System.Data;

namespace backend.Features.Auth.Persistence;

public sealed class AuthRepository(IDbConnection connection) : IAuthRepository
{
    private IDbConnection Connection => connection;

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken)
    {
        const string sql = "SELECT EXISTS(SELECT 1 FROM iam.users WHERE email = @Email AND deleted_at IS NULL)";
        return await Connection.ExecuteScalarAsync<bool>(new CommandDefinition(sql, new { Email = email }, cancellationToken: cancellationToken));
    }

    public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT u.id, u.public_id, u.full_name, u.email, ui.password_hash, u.status, u.email_verified_at, u.created_at, u.last_login_at
            FROM iam.users u
            JOIN iam.user_identities ui ON ui.user_id = u.id AND ui.provider = 'local'
            WHERE u.email = @Email AND u.deleted_at IS NULL
            """;
        return await Connection.QuerySingleOrDefaultAsync<User>(new CommandDefinition(sql, new { Email = email }, cancellationToken: cancellationToken));
    }

    public async Task<User> CreateUserAsync(string fullName, string email, string passwordHash, CancellationToken cancellationToken)
    {
        const string sql = """
            WITH new_user AS (
                INSERT INTO iam.users (full_name, email)
                VALUES (@FullName, @Email)
                RETURNING id, public_id, full_name, email, status, email_verified_at, created_at, last_login_at
            ),
            new_identity AS (
                INSERT INTO iam.user_identities (user_id, provider, provider_subject, password_hash, verified_at)
                SELECT nu.id, 'local', nu.email, @PasswordHash, NOW()
                FROM new_user nu
                RETURNING password_hash
            )
            SELECT nu.id, nu.public_id, nu.full_name, nu.email, ni.password_hash, nu.status, nu.email_verified_at, nu.created_at, nu.last_login_at
            FROM new_user nu
            CROSS JOIN new_identity ni
            """;
        return await Connection.QuerySingleAsync<User>(new CommandDefinition(sql, new { FullName = fullName, Email = email, PasswordHash = passwordHash }, cancellationToken: cancellationToken));
    }

    public async Task<Session> CreateSessionAsync(long userId, string refreshTokenHash, DateTimeOffset expiresAt, CancellationToken cancellationToken)
    {
        const string sql = """
            INSERT INTO iam.user_sessions (user_id, refresh_token_hash, expires_at)
            VALUES (@UserId, @RefreshTokenHash, @ExpiresAt)
            RETURNING id, public_id, user_id, refresh_token_hash, expires_at, created_at
            """;
        return await Connection.QuerySingleAsync<Session>(new CommandDefinition(sql, new { UserId = userId, RefreshTokenHash = refreshTokenHash, ExpiresAt = expiresAt }, cancellationToken: cancellationToken));
    }

    public async Task<(User User, Session Session)?> RotateSessionAsync(string oldRefreshTokenHash, string newRefreshTokenHash, DateTimeOffset newExpiresAt, CancellationToken cancellationToken)
    {
        const string sql = """
            WITH updated_session AS (
                UPDATE iam.user_sessions
                SET refresh_token_hash = @NewRefreshTokenHash, expires_at = @NewExpiresAt
                WHERE refresh_token_hash = @OldRefreshTokenHash AND expires_at > NOW() AND revoked_at IS NULL
                RETURNING id, public_id, user_id, refresh_token_hash, expires_at, created_at
            ),
            updated_user AS (
                UPDATE iam.users
                SET last_login_at = NOW()
                WHERE id = (SELECT user_id FROM updated_session)
                RETURNING id, public_id, full_name, email, status, email_verified_at, created_at, last_login_at
            )
            SELECT
                u.id, u.public_id, u.full_name, u.email, '' AS password_hash, u.status, u.email_verified_at, u.created_at, u.last_login_at,
                s.id, s.public_id, s.user_id, s.refresh_token_hash, s.expires_at, s.created_at
            FROM updated_user u
            JOIN updated_session s ON s.user_id = u.id
            """;

        var result = await Connection.QueryAsync<User, Session, (User, Session)>(
            new CommandDefinition(sql, new { OldRefreshTokenHash = oldRefreshTokenHash, NewRefreshTokenHash = newRefreshTokenHash, NewExpiresAt = newExpiresAt }, cancellationToken: cancellationToken),
            (user, session) => (user, session));

        return result.SingleOrDefault();
    }

    public async Task<User?> GetUserBySessionAsync(Guid sessionPublicId, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT u.id, u.public_id, u.full_name, u.email, '' AS password_hash, u.status, u.email_verified_at, u.created_at, u.last_login_at
            FROM iam.users u
            JOIN iam.user_sessions s ON s.user_id = u.id
            WHERE s.public_id = @SessionPublicId AND s.expires_at > NOW() AND s.revoked_at IS NULL AND u.deleted_at IS NULL
            """;
        return await Connection.QuerySingleOrDefaultAsync<User>(new CommandDefinition(sql, new { SessionPublicId = sessionPublicId }, cancellationToken: cancellationToken));
    }

    public async Task<bool> RevokeSessionAsync(Guid sessionPublicId, CancellationToken cancellationToken)
    {
        const string sql = """
            UPDATE iam.user_sessions
            SET revoked_at = NOW(), expires_at = NOW(), revoke_reason = 'logout'
            WHERE public_id = @SessionPublicId AND revoked_at IS NULL
            """;
        var affected = await Connection.ExecuteAsync(new CommandDefinition(sql, new { SessionPublicId = sessionPublicId }, cancellationToken: cancellationToken));
        return affected > 0;
    }
}
