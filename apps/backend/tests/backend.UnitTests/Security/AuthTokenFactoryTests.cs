using System.Text.RegularExpressions;
using backend.Features.Auth.Services;

namespace backend.UnitTests.Security;

public sealed partial class AuthTokenFactoryTests
{
    [Fact]
    public void GenerateRefreshToken_uses_the_expected_prefix_and_shape()
    {
        var token = PasswordService.GenerateRefreshToken();

        Assert.Matches(TokenPattern("^"), token);
    }

    private static Regex TokenPattern(string prefixPattern) =>
        new($"{prefixPattern}[0-9a-f]{{64}}$", RegexOptions.CultureInvariant);
}
