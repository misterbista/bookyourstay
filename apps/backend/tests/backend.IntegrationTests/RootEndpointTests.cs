using System;
using System.Text.Json;
using System.Threading.Tasks;
using backend.IntegrationTests.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;

namespace backend.IntegrationTests;

public sealed class RootEndpointTests(BackendWebApplicationFactory factory)
    : IClassFixture<BackendWebApplicationFactory>
{
    [Fact]
    public async Task GetRoot_returns_api_metadata()
    {
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });

        var response = await client.GetAsync("/");

        response.EnsureSuccessStatusCode();

        await using var responseStream = await response.Content.ReadAsStreamAsync();
        using var document = await JsonDocument.ParseAsync(responseStream);
        var root = document.RootElement;

        Assert.Equal("BookYourStay API", root.GetProperty("name").GetString());
        Assert.Equal("v1", root.GetProperty("version").GetString());
        Assert.Equal("micro-feature", root.GetProperty("architecture").GetString());
    }
}
