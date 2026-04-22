using System.Security.Cryptography;
using Microsoft.AspNetCore.DataProtection;

namespace backend.Features.Auth.Security;

public sealed class AuthCookieTokenProtector
{
    private readonly IDataProtector _accessTokenProtector;
    private readonly IDataProtector _refreshTokenProtector;

    public AuthCookieTokenProtector(IDataProtectionProvider dataProtectionProvider)
    {
        _accessTokenProtector = dataProtectionProvider.CreateProtector("BookYourStay.Auth.AccessTokenCookie.v1");
        _refreshTokenProtector = dataProtectionProvider.CreateProtector("BookYourStay.Auth.RefreshTokenCookie.v1");
    }

    public string ProtectAccessToken(string token) => _accessTokenProtector.Protect(token);

    public string ProtectRefreshToken(string token) => _refreshTokenProtector.Protect(token);

    public string? UnprotectAccessToken(string protectedToken) =>
        Unprotect(_accessTokenProtector, protectedToken);

    private static string? Unprotect(IDataProtector protector, string protectedToken)
    {
        try
        {
            return protector.Unprotect(protectedToken);
        }
        catch (CryptographicException)
        {
            return null;
        }
    }
}
