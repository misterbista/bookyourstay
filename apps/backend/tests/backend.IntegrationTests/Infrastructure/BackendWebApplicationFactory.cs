using System.Collections.Generic;
using backend.Features.Auth.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace backend.IntegrationTests.Infrastructure;

public sealed class BackendWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, configurationBuilder) =>
        {
            configurationBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Host=localhost;Port=5432;Database=bookyourstay;Username=bookyourstay;Password=bookyourstay",
                ["Database:RunMigrationsOnStartup"] = "false",
                ["Jwt:SecretKey"] = "bookyourstay-testing-jwt-secret-key-2026"
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IAuthRepository>();
            services.AddSingleton<IAuthRepository, FakeAuthRepository>();
        });
    }
}
