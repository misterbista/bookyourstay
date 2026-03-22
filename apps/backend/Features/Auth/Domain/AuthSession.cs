namespace backend.Features.Auth.Domain;

public sealed class AuthSession
{
    public long Id { get; init; }

    public Guid PublicId { get; init; } = Guid.NewGuid();

    public long UserId { get; init; }

    public string RefreshTokenHash { get; init; } = string.Empty;

    public DateTimeOffset AccessTokenExpiresAt { get; init; }

    public string? DeviceName { get; init; }

    public string? IpAddress { get; init; }

    public string? UserAgent { get; init; }

    public DateTimeOffset? LastUsedAt { get; set; }

    public DateTimeOffset ExpiresAt { get; init; }

    public DateTimeOffset? RevokedAt { get; set; }

    public string? RevokeReason { get; set; }

    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
