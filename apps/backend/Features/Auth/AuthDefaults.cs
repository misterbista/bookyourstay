namespace backend.Features.Auth;

internal static class AuthDefaults
{
    public static readonly TimeSpan SessionLifetime = TimeSpan.FromDays(7);
    public static readonly TimeSpan PasswordResetTokenLifetime = TimeSpan.FromMinutes(15);
}
