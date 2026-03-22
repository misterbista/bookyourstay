namespace backend.Features.Auth;

internal static class HttpContextAuthExtensions
{
    public static string? GetBearerToken(this HttpContext httpContext)
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
}
