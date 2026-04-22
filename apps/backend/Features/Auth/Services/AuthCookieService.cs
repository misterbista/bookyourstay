using Microsoft.AspNetCore.DataProtection;

namespace backend.Features.Auth.Services;

public sealed class AuthCookieService
{
    private const string AccessTokenCookieName = "byst_at";
    private const string RefreshTokenCookieName = "byst_rt";

    private readonly IDataProtector _accessTokenProtector;
    private readonly IDataProtector _refreshTokenProtector;

    public AuthCookieService(IDataProtectionProvider dataProtectionProvider)
    {
        _accessTokenProtector = dataProtectionProvider.CreateProtector("BookYourStay.Auth.AccessToken.v1");
        _refreshTokenProtector = dataProtectionProvider.CreateProtector("BookYourStay.Auth.RefreshToken.v1");
    }

    public string? GetAccessToken(HttpContext httpContext) =>
        ReadProtectedCookie(httpContext, AccessTokenCookieName, _accessTokenProtector);

    public string? GetRefreshToken(HttpContext httpContext) =>
        ReadProtectedCookie(httpContext, RefreshTokenCookieName, _refreshTokenProtector);

    public void SetAuthCookies(
        HttpContext httpContext,
        string accessToken,
        DateTimeOffset accessTokenExpiry,
        string refreshToken,
        DateTimeOffset refreshTokenExpiry)
    {
        httpContext.Response.Cookies.Append(
            AccessTokenCookieName,
            _accessTokenProtector.Protect(accessToken),
            CreateCookieOptions(httpContext, accessTokenExpiry));

        httpContext.Response.Cookies.Append(
            RefreshTokenCookieName,
            _refreshTokenProtector.Protect(refreshToken),
            CreateCookieOptions(httpContext, refreshTokenExpiry));
    }

    public void ClearAuthCookies(HttpContext httpContext)
    {
        var options = CreateCookieOptions(httpContext, DateTimeOffset.UtcNow.AddDays(-1));

        httpContext.Response.Cookies.Delete(AccessTokenCookieName, options);
        httpContext.Response.Cookies.Delete(RefreshTokenCookieName, options);
    }

    private static CookieOptions CreateCookieOptions(HttpContext httpContext, DateTimeOffset expiresAt) =>
        new()
        {
            HttpOnly = true,
            Secure = httpContext.Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            Expires = expiresAt
        };

    private static string? ReadProtectedCookie(HttpContext httpContext, string cookieName, IDataProtector protector)
    {
        var protectedToken = httpContext.Request.Cookies[cookieName];
        if (string.IsNullOrWhiteSpace(protectedToken))
        {
            return null;
        }

        try
        {
            return protector.Unprotect(protectedToken);
        }
        catch
        {
            return null;
        }
    }
}
