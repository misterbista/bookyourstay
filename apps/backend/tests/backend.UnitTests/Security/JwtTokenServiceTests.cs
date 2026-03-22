using System;
using backend.Features.Auth.Domain;
using backend.Features.Auth.Security;
using Microsoft.Extensions.Options;

namespace backend.UnitTests.Security;

public sealed class JwtTokenServiceTests
{
    [Fact]
    public void CreateAccessToken_can_be_validated_back_into_the_expected_payload()
    {
        var jwtOptions = Options.Create(new JwtOptions
        {
            Issuer = "BookYourStay.Tests",
            Audience = "BookYourStay.TestsClient",
            SecretKey = "bookyourstay-tests-secret-key-1234567890",
            AccessTokenLifetimeMinutes = 30
        });

        var timeProvider = TimeProvider.System;
        var service = new JwtTokenService(jwtOptions, timeProvider);

        var user = new AuthUser
        {
            Id = 42,
            PublicId = Guid.NewGuid(),
            FullName = "Test User",
            Email = "test@example.com",
            Status = AuthUserStatuses.Active,
            CreatedAt = timeProvider.GetUtcNow(),
            UpdatedAt = timeProvider.GetUtcNow()
        };

        var session = new AuthSession
        {
            Id = 7,
            PublicId = Guid.NewGuid(),
            UserId = user.Id,
            RefreshTokenHash = "hash",
            ExpiresAt = timeProvider.GetUtcNow().AddDays(7),
            CreatedAt = timeProvider.GetUtcNow(),
            UpdatedAt = timeProvider.GetUtcNow()
        };

        var accessToken = service.CreateAccessToken(user, session);

        var isValid = service.TryValidateAccessToken(accessToken.Token, out var payload);

        Assert.True(isValid);
        Assert.Equal(user.Id, payload.UserId);
        Assert.Equal(user.PublicId, payload.UserPublicId);
        Assert.Equal(session.PublicId, payload.SessionPublicId);
    }
}
