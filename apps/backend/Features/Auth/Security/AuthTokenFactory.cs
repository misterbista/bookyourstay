using System.Security.Cryptography;

namespace backend.Features.Auth.Security;

public static class AuthTokenFactory
{
    public static string CreateRefreshToken() => CreateToken("byst_rt");

    public static string CreatePasswordResetToken() => CreateToken("rst");

    private static string CreateToken(string prefix) =>
        $"{prefix}_{Convert.ToHexStringLower(RandomNumberGenerator.GetBytes(32))}";
}
