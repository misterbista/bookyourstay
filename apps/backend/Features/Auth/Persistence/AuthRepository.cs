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
        CancellationToken cancellationToken)
    {
        const string insertUserSql = """
            INSERT INTO iam.users (full_name, email, status)
            VALUES (@FullName, @Email, @Status)
            RETURNING
                id AS "Id",
                public_id AS "PublicId",
                full_name AS "FullName",
                email AS "Email",
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
            INSERT INTO iam.user_identities (user_id, provider, provider_subject, password_hash, provider_email)
            VALUES (@UserId, @Provider, @ProviderSubject, @PasswordHash, @ProviderEmail);
            """;

        const string insertSessionSql = """
            INSERT INTO iam.user_sessions (user_id, refresh_token_hash, expires_at)
            VALUES (@UserId, @RefreshTokenHash, @ExpiresAt)
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

        var user = await Connection.QuerySingleAsync<AuthUser>(new CommandDefinition(
            insertUserSql,
            new
            {
                FullName = fullName,
                Email = normalizedEmail,
                Status = userStatus
            },
            cancellationToken: cancellationToken));

        await Connection.ExecuteAsync(new CommandDefinition(
            insertIdentitySql,
            new
            {
                UserId = user.Id,
                Provider = AuthIdentityProviders.Local,
                ProviderSubject = normalizedEmail,
                PasswordHash = passwordHash,
                ProviderEmail = normalizedEmail
            },
            cancellationToken: cancellationToken));

        var session = await Connection.QuerySingleAsync<AuthSession>(new CommandDefinition(
            insertSessionSql,
            new
            {
                UserId = user.Id,
                RefreshTokenHash = refreshTokenHash,
                ExpiresAt = sessionExpiresAt
            },
            cancellationToken: cancellationToken));

        return (user, session);
    }

    public async Task<(AuthUser User, AuthIdentity Identity)?> GetLocalIdentityByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT
                u.id AS "UserId",
                u.public_id AS "UserPublicId",
                u.full_name AS "UserFullName",
                u.email AS "UserEmail",
                u.phone AS "UserPhone",
                u.status AS "UserStatus",
                u.email_verified_at AS "UserEmailVerifiedAt",
                u.phone_verified_at AS "UserPhoneVerifiedAt",
                u.created_at AS "UserCreatedAt",
                u.updated_at AS "UserUpdatedAt",
                u.last_login_at AS "UserLastLoginAt",
                u.deleted_at AS "UserDeletedAt",
                i.id AS "IdentityId",
                i.user_id AS "IdentityUserId",
                i.provider AS "IdentityProvider",
                i.provider_subject AS "IdentityProviderSubject",
                i.password_hash AS "IdentityPasswordHash",
                i.provider_email AS "IdentityProviderEmail",
                i.provider_metadata AS "IdentityProviderMetadata",
                i.verified_at AS "IdentityVerifiedAt",
                i.last_used_at AS "IdentityLastUsedAt",
                i.created_at AS "IdentityCreatedAt"
            FROM iam.users u
            INNER JOIN iam.user_identities i ON i.user_id = u.id
            WHERE u.email = @Email
              AND u.deleted_at IS NULL
              AND i.provider = @Provider
            LIMIT 1;
            """;

        var record = await Connection.QuerySingleOrDefaultAsync<LocalAuthRecord>(new CommandDefinition(
            sql,
            new { Email = normalizedEmail, Provider = AuthIdentityProviders.Local },
            cancellationToken: cancellationToken));

        if (record is null)
        {
            return null;
        }

        var user = new AuthUser
        {
            Id = record.UserId,
            PublicId = record.UserPublicId,
            FullName = record.UserFullName,
            Email = record.UserEmail,
            Phone = record.UserPhone,
            Status = record.UserStatus,
            EmailVerifiedAt = record.UserEmailVerifiedAt,
            PhoneVerifiedAt = record.UserPhoneVerifiedAt,
            CreatedAt = record.UserCreatedAt,
            UpdatedAt = record.UserUpdatedAt,
            LastLoginAt = record.UserLastLoginAt,
            DeletedAt = record.UserDeletedAt
        };

        var identity = new AuthIdentity
        {
            Id = record.IdentityId,
            UserId = record.IdentityUserId,
            Provider = record.IdentityProvider,
            ProviderSubject = record.IdentityProviderSubject,
            PasswordHash = record.IdentityPasswordHash,
            ProviderEmail = record.IdentityProviderEmail,
            ProviderMetadata = record.IdentityProviderMetadata,
            VerifiedAt = record.IdentityVerifiedAt,
            LastUsedAt = record.IdentityLastUsedAt,
            CreatedAt = record.IdentityCreatedAt
        };

        return (user, identity);
    }

    public async Task<AuthSession> CreateSessionAsync(
        long userId,
        long identityId,
        string refreshTokenHash,
        DateTimeOffset sessionExpiresAt,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        const string sql = """
            UPDATE iam.users
            SET last_login_at = @Now,
                updated_at = @Now
            WHERE id = @UserId;

            UPDATE iam.user_identities
            SET last_used_at = @Now
            WHERE id = @IdentityId;

            INSERT INTO iam.user_sessions (user_id, refresh_token_hash, expires_at, last_used_at)
            VALUES (@UserId, @RefreshTokenHash, @ExpiresAt, @Now)
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

        return await Connection.QuerySingleAsync<AuthSession>(new CommandDefinition(
            sql,
            new
            {
                UserId = userId,
                RefreshTokenHash = refreshTokenHash,
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
                    u.email AS "Email",
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
                t.id AS "TicketId",
                t.user_id AS "TicketUserId",
                t.token_hash AS "TicketTokenHash",
                t.expires_at AS "TicketExpiresAt",
                t.consumed_at AS "TicketConsumedAt",
                t.created_at AS "TicketCreatedAt",
                u.id AS "UserId",
                u.public_id AS "UserPublicId",
                u.full_name AS "UserFullName",
                u.email AS "UserEmail",
                u.phone AS "UserPhone",
                u.status AS "UserStatus",
                u.email_verified_at AS "UserEmailVerifiedAt",
                u.phone_verified_at AS "UserPhoneVerifiedAt",
                u.created_at AS "UserCreatedAt",
                u.updated_at AS "UserUpdatedAt",
                u.last_login_at AS "UserLastLoginAt",
                u.deleted_at AS "UserDeletedAt",
                i.id AS "IdentityId",
                i.user_id AS "IdentityUserId",
                i.provider AS "IdentityProvider",
                i.provider_subject AS "IdentityProviderSubject",
                i.password_hash AS "IdentityPasswordHash",
                i.provider_email AS "IdentityProviderEmail",
                i.provider_metadata AS "IdentityProviderMetadata",
                i.verified_at AS "IdentityVerifiedAt",
                i.last_used_at AS "IdentityLastUsedAt",
                i.created_at AS "IdentityCreatedAt"
            FROM iam.password_reset_tokens t
            INNER JOIN iam.users u ON u.id = t.user_id
            INNER JOIN iam.user_identities i ON i.user_id = u.id
            WHERE t.token_hash = @TokenHash
              AND i.provider = @Provider
            LIMIT 1;
            """;

        var record = await Connection.QuerySingleOrDefaultAsync<PasswordResetContextRecord>(new CommandDefinition(
            sql,
            new { TokenHash = tokenHash, Provider = AuthIdentityProviders.Local },
            cancellationToken: cancellationToken));
        if (record is null)
        {
            return null;
        }

        var ticket = new PasswordResetTicket
        {
            Id = record.TicketId,
            UserId = record.TicketUserId,
            TokenHash = record.TicketTokenHash,
            ExpiresAt = record.TicketExpiresAt,
            ConsumedAt = record.TicketConsumedAt,
            CreatedAt = record.TicketCreatedAt
        };

        var user = new AuthUser
        {
            Id = record.UserId,
            PublicId = record.UserPublicId,
            FullName = record.UserFullName,
            Email = record.UserEmail,
            Phone = record.UserPhone,
            Status = record.UserStatus,
            EmailVerifiedAt = record.UserEmailVerifiedAt,
            PhoneVerifiedAt = record.UserPhoneVerifiedAt,
            CreatedAt = record.UserCreatedAt,
            UpdatedAt = record.UserUpdatedAt,
            LastLoginAt = record.UserLastLoginAt,
            DeletedAt = record.UserDeletedAt
        };

        var identity = new AuthIdentity
        {
            Id = record.IdentityId,
            UserId = record.IdentityUserId,
            Provider = record.IdentityProvider,
            ProviderSubject = record.IdentityProviderSubject,
            PasswordHash = record.IdentityPasswordHash,
            ProviderEmail = record.IdentityProviderEmail,
            ProviderMetadata = record.IdentityProviderMetadata,
            VerifiedAt = record.IdentityVerifiedAt,
            LastUsedAt = record.IdentityLastUsedAt,
            CreatedAt = record.IdentityCreatedAt
        };

        return new PasswordResetContext(user, identity, ticket);
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
                  AND EXISTS (SELECT 1 FROM consumed_token)
                RETURNING id
            )
            SELECT EXISTS (SELECT 1 FROM consumed_token)
                AND EXISTS (SELECT 1 FROM updated_user)
                AND EXISTS (SELECT 1 FROM updated_identity);
            """;

        return await Connection.ExecuteScalarAsync<bool>(new CommandDefinition(
            sql,
            new { UserId = userId, IdentityId = identityId, PasswordHash = passwordHash, TokenHash = tokenHash, Now = now },
            cancellationToken: cancellationToken));
    }

    private sealed class LocalAuthRecord
    {
        public long UserId { get; init; }
        public Guid UserPublicId { get; init; }
        public string UserFullName { get; init; } = string.Empty;
        public string? UserEmail { get; init; }
        public string? UserPhone { get; init; }
        public string UserStatus { get; init; } = string.Empty;
        public DateTimeOffset? UserEmailVerifiedAt { get; init; }
        public DateTimeOffset? UserPhoneVerifiedAt { get; init; }
        public DateTimeOffset UserCreatedAt { get; init; }
        public DateTimeOffset UserUpdatedAt { get; init; }
        public DateTimeOffset? UserLastLoginAt { get; init; }
        public DateTimeOffset? UserDeletedAt { get; init; }
        public long IdentityId { get; init; }
        public long IdentityUserId { get; init; }
        public string IdentityProvider { get; init; } = string.Empty;
        public string IdentityProviderSubject { get; init; } = string.Empty;
        public string? IdentityPasswordHash { get; init; }
        public string? IdentityProviderEmail { get; init; }
        public string? IdentityProviderMetadata { get; init; }
        public DateTimeOffset? IdentityVerifiedAt { get; init; }
        public DateTimeOffset? IdentityLastUsedAt { get; init; }
        public DateTimeOffset IdentityCreatedAt { get; init; }
    }

    private sealed class PasswordResetContextRecord
    {
        public long TicketId { get; init; }
        public long TicketUserId { get; init; }
        public string TicketTokenHash { get; init; } = string.Empty;
        public DateTimeOffset TicketExpiresAt { get; init; }
        public DateTimeOffset? TicketConsumedAt { get; init; }
        public DateTimeOffset TicketCreatedAt { get; init; }
        public long UserId { get; init; }
        public Guid UserPublicId { get; init; }
        public string UserFullName { get; init; } = string.Empty;
        public string? UserEmail { get; init; }
        public string? UserPhone { get; init; }
        public string UserStatus { get; init; } = string.Empty;
        public DateTimeOffset? UserEmailVerifiedAt { get; init; }
        public DateTimeOffset? UserPhoneVerifiedAt { get; init; }
        public DateTimeOffset UserCreatedAt { get; init; }
        public DateTimeOffset UserUpdatedAt { get; init; }
        public DateTimeOffset? UserLastLoginAt { get; init; }
        public DateTimeOffset? UserDeletedAt { get; init; }
        public long IdentityId { get; init; }
        public long IdentityUserId { get; init; }
        public string IdentityProvider { get; init; } = string.Empty;
        public string IdentityProviderSubject { get; init; } = string.Empty;
        public string? IdentityPasswordHash { get; init; }
        public string? IdentityProviderEmail { get; init; }
        public string? IdentityProviderMetadata { get; init; }
        public DateTimeOffset? IdentityVerifiedAt { get; init; }
        public DateTimeOffset? IdentityLastUsedAt { get; init; }
        public DateTimeOffset IdentityCreatedAt { get; init; }
    }
}
