using backend.Features.Auth.Contracts;
using backend.Features.Auth.Services;
using backend.Shared.Contracts;
using backend.Shared.Http;
using backend.Shared.Results;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.Auth.Http;

public static class AuthResultExtensions
{
    public static IActionResult ToAuthActionResult(
        this ControllerBase controller,
        ApplicationResult<AuthResponse> result,
        AuthCookieService authCookies)
    {
        if (!result.Success || result.Data is null)
        {
            return controller.ToActionResult(result);
        }

        authCookies.SetAuthCookies(
            controller.HttpContext,
            result.Data.AccessToken,
            result.Data.AccessTokenExpiresAt,
            result.Data.RefreshToken,
            result.Data.RefreshTokenExpiresAt);

        var response = new AuthSessionResponse(result.Data.SessionId);
        return controller.StatusCode(
            result.StatusCode,
            new ApiResponse<AuthSessionResponse>(true, result.Message, response, result.Meta));
    }
}
