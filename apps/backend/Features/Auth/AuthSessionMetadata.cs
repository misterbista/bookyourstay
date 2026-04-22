namespace backend.Features.Auth;

public sealed record AuthSessionMetadata(
    string? DeviceName,
    string? IpAddress,
    string? UserAgent);
