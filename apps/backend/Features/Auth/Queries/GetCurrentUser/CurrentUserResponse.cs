namespace backend.Features.Auth.Queries.GetCurrentUser;

public sealed record CurrentUserResponse(
    Guid UserId,
    string FullName,
    string Email,
    string Status,
    DateTimeOffset? EmailVerifiedAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset? LastLoginAt);
