using backend.Features.Auth.Options;
using backend.Features.Auth.Persistence;
using backend.Features.Auth.Security;
using backend.Features.Auth.Services;
using Microsoft.AspNetCore.Authentication;

namespace backend.Features.Auth;

public static class AuthFeature
{
    public const string AuthenticationScheme = "CookieAccessToken";

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
        services.AddScoped<AuthService>();
        services.AddScoped<AuthSessionService>();

        services
            .AddAuthentication(AuthenticationScheme)
            .AddScheme<AuthenticationSchemeOptions, CookieAccessTokenAuthenticationHandler>(
                AuthenticationScheme,
                _ => { });

        services.AddAuthorization();

        return services;
    }
}
