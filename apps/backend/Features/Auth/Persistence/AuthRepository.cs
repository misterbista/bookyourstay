using Dapper;
using backend.Features.Auth.Domain;
using System.Data;

namespace backend.Features.Auth.Persistence;

public sealed class AuthRepository(IDbConnection connection)
{
    private IDbConnection Connection => connection;

    public async Task<bool> EmailExistsAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT EXISTS (
                SELECT 1
                FROM iam.users
                WHERE email = @Email
                  AND deleted_at IS NULL
            );
            """;

        return await Connection.ExecuteScalarAsync<bool>(new CommandDefinition(
            sql,
            new { Email = normalizedEmail },
            cancellationToken: cancellationToken));
    }

    public async Task<(AuthUser User, AuthSession Session)> RegisterLocalUserAsync(
        string fullName,
        string normalizedEmail,
        string passwordHash,
        string userStatus,
        string refreshTokenHash,
        DateTimeOffset sessionExpiresAt,
        DateTimeOffset now,
        AuthSessionMetadata sessionMetadata,
        CancellationToken cancellationToken)
    {
        const string insertUserSql = """
            INSERT INTO iam.users (full_name, email, status, last_login_at, updated_at)
            VALUES (@FullName, @Email, @Status, @Now, @Now)
            RETURNING
                id AS "Id",
                public_id AS "PublicId",
                full_name AS "FullName",
                email::text AS "Email",
                phone AS "Phone",
                status AS "Status",
                email_verified_at AS "EmailVerifiedAt",
                phone_verified_at AS "PhoneVerifiedAt",
                created_at AS "CreatedAt",
                updated_at AS "UpdatedAt",
                last_login_at AS "LastLoginAt",
                deleted_at AS "DeletedAt";
            """;

        const string insertIdentitySql = """
            INSERT INTO iam.user_identities (user_id, provider, provider_subject, password_hash, provider_email, last_used_at)
            VALUES (@UserId, @Provider, @ProviderSubject, @PasswordHash, @ProviderEmail, @Now);
            """;

        const string insertSessionSql = """
            INSERT INTO iam.user_sessions (
                user_id,
                refresh_token_hash,
                device_name,
                ip_address,
                user_agent,
                expires_at,
                last_used_at
            )
            VALUES (
                @UserId,
                @RefreshTokenHash,
                @DeviceName,
                NULLIF(@IpAddress, '')::inet,
                @UserAgent,
                @ExpiresAt,
                @Now
            )
            RETURNING
                id AS "Id",
                public_id AS "PublicId",
                user_id AS "UserId",
                refresh_token_hash AS "RefreshTokenHash",
                device_name AS "DeviceName",
                ip_address::text AS "IpAddress",
                user_agent AS "UserAgent",
                last_used_at AS "LastUsedAt",
                expires_at AS "ExpiresAt",
                revoked_at AS "RevokedAt",
                revoke_reason AS "RevokeReason",
                created_at AS "CreatedAt",
                updated_at AS "UpdatedAt";
            """;

        var shouldCloseConnection = Connection.State == ConnectionState.Closed;
        if (shouldCloseConnection)
        {
            Connection.Open();
        }

        using var transaction = Connection.BeginTransaction();
        try
        {
            var user = await Connection.QuerySingleAsync<AuthUser>(new CommandDefinition(
                insertUserSql,
                new
                {
                    FullName = fullName,
                    Email = normalizedEmail,
                    Status = userStatus,
                    Now = now
                },
                transaction,
                cancellationToken: cancellationToken));

            await Connection.ExecuteAsync(new CommandDefinition(
                insertIdentitySql,
                new
                {
                    UserId = user.Id,
                    Provider = AuthIdentityProviders.Local,
                    ProviderSubject = normalizedEmail,
                    PasswordHash = passwordHash,
                    ProviderEmail = normalizedEmail,
                    Now = now
                },
                transaction,
                cancellationToken: cancellationToken));

            var session = await Connection.QuerySingleAsync<AuthSession>(new CommandDefinition(
                insertSessionSql,
                new
                {
                    UserId = user.Id,
                    RefreshTokenHash = refreshTokenHash,
                    sessionMetadata.DeviceName,
                    sessionMetadata.IpAddress,
                    sessionMetadata.UserAgent,
                    ExpiresAt = sessionExpiresAt,
                    Now = now
                },
                transaction,
                cancellationToken: cancellationToken));

            transaction.Commit();
            return (user, session);
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
        finally
        {
            if (shouldCloseConnection)
            {
                Connection.Close();
            }
        }
    }

    public async Task<(AuthUser User, AuthIdentity Identity)?> GetLocalIdentityByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                u.id AS "Id",
                u.public_id AS "PublicId",
                u.full_name AS "FullName",
                u.email::text AS "Email",
                u.phone AS "Phone",
                u.status AS "Status",
                u.email_verified_at AS "EmailVerifiedAt",
                u.phone_verified_at AS "PhoneVerifiedAt",
                u.created_at AS "CreatedAt",
                u.updated_at AS "UpdatedAt",
                u.last_login_at AS "LastLoginAt",
                u.deleted_at AS "DeletedAt",
                i.id AS "IdentitySplit",
                i.id AS "Id",
                i.user_id AS "UserId",
                i.provider AS "Provider",
                i.provider_subject AS "ProviderSubject",
                i.password_hash AS "PasswordHash",
                i.provider_email::text AS "ProviderEmail",
                i.provider_metadata::text AS "ProviderMetadata",
                i.verified_at AS "VerifiedAt",
                i.last_used_at AS "LastUsedAt",
                i.created_at AS "CreatedAt"
            FROM iam.users u
            INNER JOIN iam.user_identities i ON i.user_id = u.id
            WHERE u.email = @Email
              AND u.deleted_at IS NULL
              AND i.provider = @Provider
            LIMIT 1;
            """;

        var records = await Connection.QueryAsync<AuthUser, AuthIdentity, (AuthUser User, AuthIdentity Identity)>(
            new CommandDefinition(
                sql,
                new { Email = normalizedEmail, Provider = AuthIdentityProviders.Local },
                cancellationToken: cancellationToken),
            (user, identity) => (user, identity),
            splitOn: "IdentitySplit");

        return records.SingleOrDefault();
    }

    public async Task<AuthSession> CreateSessionAsync(
        long userId,
        long identityId,
        string refreshTokenHash,
        DateTimeOffset sessionExpiresAt,
        DateTimeOffset now,
        AuthSessionMetadata sessionMetadata,
        CancellationToken cancellationToken)
    {
        const string sql = """
            WITH updated_user AS (
                UPDATE iam.users
                SET last_login_at = @Now,
                    updated_at = @Now
                WHERE id = @UserId
                  AND deleted_at IS NULL
                RETURNING id
            ),
            updated_identity AS (
                UPDATE iam.user_identities
                SET last_used_at = @Now
                WHERE id = @IdentityId
                  AND user_id = @UserId
                  AND provider = @Provider
                RETURNING id
            ),
            inserted_session AS (
                INSERT INTO iam.user_sessions (
                    user_id,
                    refresh_token_hash,
                    device_name,
                    ip_address,
                    user_agent,
                    expires_at,
                    last_used_at
                )
                SELECT
                    @UserId,
                    @RefreshTokenHash,
                    @DeviceName,
                    NULLIF(@IpAddress, '')::inet,
                    @UserAgent,
                    @ExpiresAt,
                    @Now
                WHERE EXISTS (SELECT 1 FROM updated_user)
                  AND EXISTS (SELECT 1 FROM updated_identity)
                RETURNING
                    id AS "Id",
                    public_id AS "PublicId",
                    user_id AS "UserId",
                    refresh_token_hash AS "RefreshTokenHash",
                    device_name AS "DeviceName",
                    ip_address::text AS "IpAddress",
                    user_agent AS "UserAgent",
                    last_used_at AS "LastUsedAt",
                    expires_at AS "ExpiresAt",
                    revoked_at AS "RevokedAt",
                    revoke_reason AS "RevokeReason",
                    created_at AS "CreatedAt",
                    updated_at AS "UpdatedAt"
            )
            SELECT *
            FROM inserted_session;
            """;

        return await Connection.QuerySingleAsync<AuthSession>(new CommandDefinition(
            sql,
            new
            {
                UserId = userId,
                IdentityId = identityId,
                Provider = AuthIdentityProviders.Local,
                RefreshTokenHash = refreshTokenHash,
                sessionMetadata.DeviceName,
                sessionMetadata.IpAddress,
                sessionMetadata.UserAgent,
                ExpiresAt = sessionExpiresAt,
                Now = now
            },
            cancellationToken: cancellationToken));
    }

    public async Task<bool> RevokeSessionAsync(Guid sessionPublicId, string reason, DateTimeOffset now, CancellationToken cancellationToken)
    {
        const string sql = """
            UPDATE iam.user_sessions
            SET revoked_at = @Now,
                revoke_reason = @Reason,
                updated_at = @Now
            WHERE public_id = @SessionPublicId
              AND revoked_at IS NULL;
            """;

        var affectedRows = await Connection.ExecuteAsync(new CommandDefinition(
            sql,
            new { SessionPublicId = sessionPublicId, Reason = reason, Now = now },
            cancellationToken: cancellationToken));

        return affectedRows > 0;
    }

    public async Task<AuthUser?> GetActiveUserBySessionAsync(Guid sessionPublicId, DateTimeOffset now, CancellationToken cancellationToken)
    {
        const string sql = """
            WITH touched_session AS (
                UPDATE iam.user_sessions s
                SET last_used_at = @Now,
                    updated_at = @Now
                FROM iam.users u
                WHERE s.public_id = @SessionPublicId
                  AND s.revoked_at IS NULL
                  AND s.expires_at > @Now
                  AND u.id = s.user_id
                  AND u.deleted_at IS NULL
                RETURNING
                    u.id AS "Id",
                    u.public_id AS "PublicId",
                    u.full_name AS "FullName",
                    u.email::text AS "Email",
                    u.phone AS "Phone",
                    u.status AS "Status",
                    u.email_verified_at AS "EmailVerifiedAt",
                    u.phone_verified_at AS "PhoneVerifiedAt",
                    u.created_at AS "CreatedAt",
                    u.updated_at AS "UpdatedAt",
                    u.last_login_at AS "LastLoginAt",
                    u.deleted_at AS "DeletedAt"
            )
            SELECT *
            FROM touched_session
            LIMIT 1;
            """;

        return await Connection.QuerySingleOrDefaultAsync<AuthUser>(new CommandDefinition(
            sql,
            new { SessionPublicId = sessionPublicId, Now = now },
            cancellationToken: cancellationToken));
    }

    public async Task StorePasswordResetTokenAsync(
        long userId,
        string tokenHash,
        DateTimeOffset expiresAt,
        CancellationToken cancellationToken)
    {
        const string sql = """
            INSERT INTO iam.password_reset_tokens (user_id, token_hash, expires_at)
            VALUES (@UserId, @TokenHash, @ExpiresAt);
            """;

        await Connection.ExecuteAsync(new CommandDefinition(
            sql,
            new { UserId = userId, TokenHash = tokenHash, ExpiresAt = expiresAt },
            cancellationToken: cancellationToken));
    }

    public async Task<PasswordResetContext?> GetPasswordResetContextAsync(string tokenHash, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                u.id AS "Id",
                u.public_id AS "PublicId",
                u.full_name AS "FullName",
                u.email::text AS "Email",
                u.phone AS "Phone",
                u.status AS "Status",
                u.email_verified_at AS "EmailVerifiedAt",
                u.phone_verified_at AS "PhoneVerifiedAt",
                u.created_at AS "CreatedAt",
                u.updated_at AS "UpdatedAt",
                u.last_login_at AS "LastLoginAt",
                u.deleted_at AS "DeletedAt",
                i.id AS "IdentitySplit",
                i.id AS "Id",
                i.user_id AS "UserId",
                i.provider AS "Provider",
                i.provider_subject AS "ProviderSubject",
                i.password_hash AS "PasswordHash",
                i.provider_email::text AS "ProviderEmail",
                i.provider_metadata::text AS "ProviderMetadata",
                i.verified_at AS "VerifiedAt",
                i.last_used_at AS "LastUsedAt",
                i.created_at AS "CreatedAt",
                t.id AS "TicketSplit",
                t.id AS "Id",
                t.user_id AS "UserId",
                t.token_hash AS "TokenHash",
                t.expires_at AS "ExpiresAt",
                t.consumed_at AS "ConsumedAt",
                t.created_at AS "CreatedAt"
            FROM iam.password_reset_tokens t
            INNER JOIN iam.users u ON u.id = t.user_id
            INNER JOIN iam.user_identities i ON i.user_id = u.id
            WHERE t.token_hash = @TokenHash
              AND i.provider = @Provider
            LIMIT 1;
            """;

        var records = await Connection.QueryAsync<AuthUser, AuthIdentity, PasswordResetTicket, PasswordResetContext>(
            new CommandDefinition(
                sql,
                new { TokenHash = tokenHash, Provider = AuthIdentityProviders.Local },
                cancellationToken: cancellationToken),
            (user, identity, ticket) => new PasswordResetContext(user, identity, ticket),
            splitOn: "IdentitySplit,TicketSplit");

        return records.SingleOrDefault();
    }

    public async Task<bool> UpdatePasswordAndConsumeResetTokenAsync(
        long userId,
        long identityId,
        string passwordHash,
        string tokenHash,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        const string sql = """
            WITH consumed_token AS (
                UPDATE iam.password_reset_tokens
                SET consumed_at = @Now
                WHERE token_hash = @TokenHash
                  AND user_id = @UserId
                  AND consumed_at IS NULL
                RETURNING user_id
            ),
            updated_user AS (
                UPDATE iam.users
                SET updated_at = @Now
                WHERE id = @UserId
                  AND EXISTS (SELECT 1 FROM consumed_token)
                RETURNING id
            ),
            updated_identity AS (
                UPDATE iam.user_identities
                SET password_hash = @PasswordHash,
                    last_used_at = @Now
                WHERE id = @IdentityId
                  AND user_id = @UserId
                  AND provider = @Provider
                  AND EXISTS (SELECT 1 FROM consumed_token)
                RETURNING id
            )
            SELECT EXISTS (SELECT 1 FROM consumed_token)
                AND EXISTS (SELECT 1 FROM updated_user)
                AND EXISTS (SELECT 1 FROM updated_identity);
            """;

        return await Connection.ExecuteScalarAsync<bool>(new CommandDefinition(
            sql,
            new
            {
                UserId = userId,
                IdentityId = identityId,
                Provider = AuthIdentityProviders.Local,
                PasswordHash = passwordHash,
                TokenHash = tokenHash,
                Now = now
            },
            cancellationToken: cancellationToken));
    }
}
