namespace backend.Features.Auth.Queries.GetCurrentUser;

public sealed record GetCurrentUserRequest;

public sealed record CurrentUserResponse(
    Guid Id,
    string FullName,
    string Email,
    string Status,
    DateTimeOffset? EmailVerifiedAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset? LastLoginAt);
