using backend.Features.Auth.Security;
using System.Net;

namespace backend.Features.Auth;

internal static class HttpContextAuthExtensions
{
    public static string? GetAccessToken(this HttpContext httpContext)
        => GetBearerToken(httpContext)
           ?? GetProtectedAccessToken(httpContext);

    private static string? GetBearerToken(HttpContext httpContext)
    {
        if (!httpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
        {
            return null;
        }

        var value = authorizationHeader.ToString();
        const string prefix = "Bearer ";

        return value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
            ? value[prefix.Length..].Trim()
            : null;
    }

    private static string? GetProtectedAccessToken(HttpContext httpContext)
    {
        var protectedToken = httpContext.Request.Cookies[AuthCookieDefaults.AccessTokenCookieName];
        if (string.IsNullOrWhiteSpace(protectedToken))
        {
            return null;
        }

        var protector = httpContext.RequestServices.GetRequiredService<AuthCookieTokenProtector>();
        return protector.UnprotectAccessToken(protectedToken);
    }

    public static AuthSessionMetadata GetAuthSessionMetadata(this HttpContext? httpContext)
    {
        var userAgent = Normalize(GetHeaderValue(httpContext, "User-Agent"), 1000);
        var deviceName = Normalize(GetHeaderValue(httpContext, "X-Device-Name"), 200)
            ?? Normalize(userAgent, 200);
        var ipAddress = GetClientIpAddress(httpContext);

        return new AuthSessionMetadata(deviceName, ipAddress, userAgent);
    }

    private static string? GetHeaderValue(HttpContext? httpContext, string name)
    {
        if (httpContext?.Request.Headers.TryGetValue(name, out var value) != true)
        {
            return null;
        }

        return value.ToString();
    }

    private static string? GetClientIpAddress(HttpContext? httpContext)
    {
        var forwardedFor = GetHeaderValue(httpContext, "X-Forwarded-For");
        if (!string.IsNullOrWhiteSpace(forwardedFor))
        {
            var firstForwardedIp = forwardedFor.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(firstForwardedIp))
            {
                return NormalizeIpAddress(firstForwardedIp);
            }
        }

        return NormalizeIpAddress(httpContext?.Connection.RemoteIpAddress?.ToString());
    }

    private static string? Normalize(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var trimmed = value.Trim();
        return trimmed.Length <= maxLength ? trimmed : trimmed[..maxLength];
    }

    private static string? NormalizeIpAddress(string? value)
    {
        return IPAddress.TryParse(value, out var ipAddress)
            ? ipAddress.ToString()
            : null;
    }
}
