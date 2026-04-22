using backend.Features.Auth.Domain;
using backend.Features.Auth.Security;

namespace backend.Features.Auth.Commands;

public sealed record AuthResponse(
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAt,
    string RefreshToken,
    DateTimeOffset RefreshTokenExpiresAt,
    Guid SessionId)
{
    public static AuthResponse From(
        JwtTokenService jwtTokenService,
        AuthUser user,
        AuthSession session,
        string refreshToken)
    {
        var accessToken = jwtTokenService.CreateAccessToken(user, session);

        return new AuthResponse(
            accessToken.Token,
            accessToken.ExpiresAt,
            refreshToken,
            session.ExpiresAt,
            session.PublicId);
    }
}

public sealed record AuthSessionResponse(
    DateTimeOffset AccessTokenExpiresAt,
    DateTimeOffset RefreshTokenExpiresAt,
    Guid SessionId)
{
    public static AuthSessionResponse From(AuthResponse response) =>
        new(
            response.AccessTokenExpiresAt,
            response.RefreshTokenExpiresAt,
            response.SessionId);
}
