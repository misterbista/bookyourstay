namespace backend.Features.Auth.Domain;

public sealed class PasswordResetTicket
{
    public long Id { get; init; }

    public long UserId { get; init; }

    public string TokenHash { get; init; } = string.Empty;

    public DateTimeOffset ExpiresAt { get; init; }

    public DateTimeOffset? ConsumedAt { get; set; }

    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
}
