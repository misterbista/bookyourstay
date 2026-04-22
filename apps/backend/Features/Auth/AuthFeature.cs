using backend.Features.Auth.Commands.Login;
using backend.Features.Auth.Commands.Logout;
using backend.Features.Auth.Commands.Refresh;
using backend.Features.Auth.Commands.Register;
using backend.Features.Auth.Options;
using backend.Features.Auth.Persistence;
using backend.Features.Auth.Queries.GetCurrentUser;
using backend.Features.Auth.Services;

namespace backend.Features.Auth;

public static class AuthFeature
{
    public static IServiceCollection AddAuthFeature(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddScoped<AuthRepository>();
        services.AddSingleton<JwtService>();
        services.AddSingleton<PasswordService>();
        services.AddSingleton<AuthCookieService>();
        services.AddScoped<LoginCommandHandler>();
        services.AddScoped<LogoutCommandHandler>();
        services.AddScoped<RefreshCommandHandler>();
        services.AddScoped<RegisterCommandHandler>();
        services.AddScoped<GetCurrentUserQueryHandler>();

        return services;
    }
}
