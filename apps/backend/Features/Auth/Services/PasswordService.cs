using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using backend.Features.Auth.Domain;

namespace backend.Features.Auth.Services;

public sealed class PasswordService
{
    private readonly PasswordHasher<User> _passwordHasher = new();

    public string HashPassword(User user, string password) =>
        _passwordHasher.HashPassword(user, password);

    public PasswordVerificationResult VerifyPassword(User user, string password) =>
        _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

    public static string GenerateRefreshToken()
    {
        return Convert.ToHexStringLower(RandomNumberGenerator.GetBytes(32));
    }

    public static string HashToken(string token)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexStringLower(bytes);
    }
}