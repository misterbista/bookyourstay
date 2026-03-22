namespace backend.Features.Auth.Domain;

public sealed class AuthIdentity
{
    public long Id { get; init; }

    public long UserId { get; init; }

    public string Provider { get; init; } = AuthIdentityProviders.Local;

    public string ProviderSubject { get; init; } = string.Empty;

    public string? PasswordHash { get; set; }

    public string? ProviderEmail { get; set; }

    public string? ProviderMetadata { get; set; }

    public DateTimeOffset? VerifiedAt { get; set; }

    public DateTimeOffset? LastUsedAt { get; set; }

    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
}
