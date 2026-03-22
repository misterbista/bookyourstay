using System.Text.RegularExpressions;
using backend.Features.Auth.Security;

namespace backend.UnitTests.Security;

public sealed partial class AuthTokenFactoryTests
{
    [Fact]
    public void CreateRefreshToken_uses_the_expected_prefix_and_shape()
    {
        var token = AuthTokenFactory.CreateRefreshToken();

        Assert.Matches(TokenPattern("^byst_rt_"), token);
    }

    [Fact]
    public void CreatePasswordResetToken_uses_the_expected_prefix_and_shape()
    {
        var token = AuthTokenFactory.CreatePasswordResetToken();

        Assert.Matches(TokenPattern("^rst_"), token);
    }

    private static Regex TokenPattern(string prefixPattern) =>
        new($"{prefixPattern}[0-9a-f]{{64}}$", RegexOptions.CultureInvariant);
}
