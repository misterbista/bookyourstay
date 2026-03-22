namespace backend.Features.Auth.Domain;

public static class AuthUserStatuses
{
    public const string Active = "active";
    public const string PendingVerification = "pending_verification";
    public const string Suspended = "suspended";
    public const string Deleted = "deleted";
}
