namespace backend.Features.Auth.Commands;

public sealed record AuthResponse(
    Guid UserId,
    string FullName,
    string Email,
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAt,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAt,
    Guid SessionId,
    string Status,
    DateTimeOffset? EmailVerifiedAt);
