using backend.Features.Auth.Security;

namespace backend.UnitTests.Security;

public sealed class TokenHasherTests
{
    [Fact]
    public void Hash_returns_a_stable_sha256_hex_value()
    {
        var hash = TokenHasher.Hash("bookyourstay");

        Assert.Equal("837B57340A4E9C5D1CFF54B1FA8133AF225E4827C3701D9E9E29654178F1E037", hash);
    }
}
