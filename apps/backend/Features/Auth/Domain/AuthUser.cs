namespace backend.Features.Auth.Domain;

public sealed class AuthUser
{
    public long Id { get; init; }

    public Guid PublicId { get; init; } = Guid.NewGuid();

    public string FullName { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string Status { get; set; } = AuthUserStatuses.PendingVerification;

    public DateTimeOffset? EmailVerifiedAt { get; set; }

    public DateTimeOffset? PhoneVerifiedAt { get; set; }

    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset? LastLoginAt { get; set; }

    public DateTimeOffset? DeletedAt { get; set; }
}
