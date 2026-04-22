using backend.Features.Auth.Services;

namespace backend.UnitTests.Security;

public sealed class TokenHasherTests
{
    [Fact]
    public void Hash_returns_a_stable_sha256_hex_value()
    {
        var hash = PasswordService.HashToken("bookyourstay");

        Assert.Equal("837b57340a4e9c5d1cff54b1fa8133af225e4827c3701d9e9e29654178f1e037", hash);
    }
}
