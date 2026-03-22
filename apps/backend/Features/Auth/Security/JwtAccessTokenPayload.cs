namespace backend.Features.Auth.Security;

public sealed record JwtAccessTokenPayload(
    long UserId,
    Guid UserPublicId,
    Guid SessionPublicId);
