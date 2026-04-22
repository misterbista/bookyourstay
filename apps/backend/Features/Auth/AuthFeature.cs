using backend.Features.Auth.Commands.Login;
using backend.Features.Auth.Commands.Logout;
using backend.Features.Auth.Commands.Refresh;
using backend.Features.Auth.Commands.Register;
using backend.Features.Auth.Options;
using backend.Features.Auth.Persistence;
using backend.Features.Auth.Queries.GetCurrentUser;
using backend.Features.Auth.Security;
using backend.Features.Auth.Services;
using Microsoft.AspNetCore.Authentication;

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

        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddSingleton<JwtService>();
        services.AddSingleton<PasswordService>();
        services.AddSingleton<AuthCookieService>();
        services.AddScoped<AuthSessionService>();
        services.AddScoped<LoginCommandHandler>();
        services.AddScoped<LogoutCommandHandler>();
        services.AddScoped<RefreshCommandHandler>();
        services.AddScoped<RegisterCommandHandler>();
        services.AddScoped<GetCurrentUserQueryHandler>();

        services
            .AddAuthentication(AuthSchemes.CookieAccessToken)
            .AddScheme<AuthenticationSchemeOptions, CookieAccessTokenAuthenticationHandler>(
                AuthSchemes.CookieAccessToken,
                _ => { });

        services.AddAuthorization();

        return services;
    }
}
