using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using backend.IntegrationTests.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;

namespace backend.IntegrationTests;

public sealed class AuthEndpointTests(BackendWebApplicationFactory factory)
    : IClassFixture<BackendWebApplicationFactory>
{
    [Fact]
    public async Task Me_requires_an_authenticated_cookie_session()
    {
        using var client = CreateClient();

        var response = await client.GetAsync("/api/v1/auth/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Register_issues_http_only_cookies_and_returns_public_session_data()
    {
        using var client = CreateClient();

        var response = await RegisterAsync(client, "register@example.com");

        await EnsureSuccessAsync(response);
        var setCookieHeaders = response.Headers.GetValues("Set-Cookie").ToArray();

        Assert.Contains(setCookieHeaders, header => header.StartsWith("byst_at=", StringComparison.Ordinal));
        Assert.Contains(setCookieHeaders, header => header.StartsWith("byst_rt=", StringComparison.Ordinal));
        Assert.All(setCookieHeaders, header => Assert.Contains("httponly", header, StringComparison.OrdinalIgnoreCase));

        using var document = await ReadJsonAsync(response);
        var root = document.RootElement;

        Assert.True(root.GetProperty("success").GetBoolean());
        Assert.True(root.GetProperty("data").TryGetProperty("sessionId", out _));
        Assert.False(root.GetProperty("data").TryGetProperty("accessToken", out _));
        Assert.False(root.GetProperty("data").TryGetProperty("refreshToken", out _));
    }

    [Fact]
    public async Task Authenticated_user_can_read_me_then_logout_revokes_the_session()
    {
        using var client = CreateClient();

        var registerResponse = await RegisterAsync(client, "me@example.com");
        await EnsureSuccessAsync(registerResponse);
        var cookieHeader = CreateCookieHeader(registerResponse);

        using var meRequest = new HttpRequestMessage(HttpMethod.Get, "/api/v1/auth/me");
        meRequest.Headers.Add("Cookie", cookieHeader);
        var meResponse = await client.SendAsync(meRequest);

        await EnsureSuccessAsync(meResponse);
        using (var document = await ReadJsonAsync(meResponse))
        {
            var data = document.RootElement.GetProperty("data");
            Assert.Equal("Test User", data.GetProperty("fullName").GetString());
            Assert.Equal("me@example.com", data.GetProperty("email").GetString());
        }

        using var logoutRequest = new HttpRequestMessage(HttpMethod.Post, "/api/v1/auth/logout");
        logoutRequest.Headers.Add("Cookie", cookieHeader);
        var logoutResponse = await client.SendAsync(logoutRequest);

        await EnsureSuccessAsync(logoutResponse);

        using var revokedMeRequest = new HttpRequestMessage(HttpMethod.Get, "/api/v1/auth/me");
        revokedMeRequest.Headers.Add("Cookie", cookieHeader);
        var revokedMeResponse = await client.SendAsync(revokedMeRequest);

        Assert.Equal(HttpStatusCode.Unauthorized, revokedMeResponse.StatusCode);
    }

    [Fact]
    public async Task Login_with_registered_credentials_issues_a_new_cookie_session()
    {
        using var client = CreateClient();

        var registerResponse = await RegisterAsync(client, "login@example.com");
        await EnsureSuccessAsync(registerResponse);

        var loginResponse = await client.PostAsJsonAsync("/api/v1/auth/login", new
        {
            email = "login@example.com",
            password = "Password123"
        });

        await EnsureSuccessAsync(loginResponse);
        var setCookieHeaders = loginResponse.Headers.GetValues("Set-Cookie").ToArray();
        Assert.Contains(setCookieHeaders, header => header.StartsWith("byst_at=", StringComparison.Ordinal));
        Assert.Contains(setCookieHeaders, header => header.StartsWith("byst_rt=", StringComparison.Ordinal));
    }

    private HttpClient CreateClient() =>
        factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

    private static Task<HttpResponseMessage> RegisterAsync(HttpClient client, string email) =>
        client.PostAsJsonAsync("/api/v1/auth/register", new
        {
            fullName = "Test User",
            email,
            password = "Password123"
        });

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
    {
        await using var responseStream = await response.Content.ReadAsStreamAsync();
        return await JsonDocument.ParseAsync(responseStream);
    }

    private static string CreateCookieHeader(HttpResponseMessage response) =>
        string.Join("; ", response.Headers.GetValues("Set-Cookie").Select(header => header.Split(';')[0]));

    private static async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var body = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException($"Expected success but got {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
    }
}
