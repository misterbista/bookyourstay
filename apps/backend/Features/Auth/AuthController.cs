using backend.Features.Auth.Commands;
using backend.Features.Auth.Commands.ForgotPassword;
using backend.Features.Auth.Commands.Login;
using backend.Features.Auth.Commands.Logout;
using backend.Features.Auth.Commands.Register;
using backend.Features.Auth.Commands.ResetPassword;
using backend.Features.Auth.Queries.GetCurrentUser;
using backend.Features.Auth.Security;
using backend.Shared.Contracts;
using EzyMediatr.Core.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.Auth;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController(
    IMediator mediator,
    AuthCookieTokenProtector cookieTokenProtector) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
        => ToAuthActionResult(await mediator.Send(request, cancellationToken));

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
        => ToAuthActionResult(await mediator.Send(request, cancellationToken));

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        if (!TryGetAccessToken(out var token, out var error))
        {
            ExpireAuthCookies();
            return error;
        }

        var result = await mediator.Send(new LogoutRequest(token), cancellationToken);
        ExpireAuthCookies();
        return ToActionResult(result);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request, CancellationToken cancellationToken)
        => ToActionResult(await mediator.Send(request, cancellationToken));

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest request, CancellationToken cancellationToken)
        => ToActionResult(await mediator.Send(request, cancellationToken));

    [HttpGet("me")]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        if (!TryGetAccessToken(out var token, out var error))
            return error;

        return ToActionResult(await mediator.Send(new GetCurrentUserRequest(token), cancellationToken));
    }

    private bool TryGetAccessToken(out string token, out IActionResult error)
    {
        var value = HttpContext.GetAccessToken();
        if (!string.IsNullOrWhiteSpace(value))
        {
            token = value;
            error = default!;
            return true;
        }

        token = default!;
        error = StatusCode(StatusCodes.Status401Unauthorized, new ApiErrorResponse(
            false,
            "Unauthorized",
            new Dictionary<string, string[]>
            {
                ["authorization"] = ["A valid auth cookie or bearer token is required."]
            }));
        return false;
    }

    private ObjectResult ToAuthActionResult(ApplicationResult<AuthResponse> result)
    {
        if (!result.Success || result.Data is null)
            return ToActionResult(result);

        AppendAuthCookies(result.Data);
        var data = result.Data;
        return StatusCode(result.StatusCode, new ApiResponse<AuthSessionResponse>(
            true,
            result.Message,
            new AuthSessionResponse(data.AccessTokenExpiresAt, data.RefreshTokenExpiresAt, data.SessionId),
            result.Meta));
    }

    private void AppendAuthCookies(AuthResponse response)
    {
        Response.Cookies.Append(
            AuthCookieDefaults.AccessTokenCookieName,
            cookieTokenProtector.ProtectAccessToken(response.AccessToken),
            CreateAuthCookieOptions(response.AccessTokenExpiresAt));

        Response.Cookies.Append(
            AuthCookieDefaults.RefreshTokenCookieName,
            cookieTokenProtector.ProtectRefreshToken(response.RefreshToken),
            CreateAuthCookieOptions(response.RefreshTokenExpiresAt));
    }

    private void ExpireAuthCookies()
    {
        var options = CreateExpiredCookieOptions();
        Response.Cookies.Delete(AuthCookieDefaults.AccessTokenCookieName, options);
        Response.Cookies.Delete(AuthCookieDefaults.RefreshTokenCookieName, options);
    }

    private CookieOptions CreateAuthCookieOptions(DateTimeOffset expiresAt) =>
        new()
        {
            HttpOnly = true,
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            Expires = expiresAt,
            Path = "/"
        };

    private CookieOptions CreateExpiredCookieOptions() =>
        new()
        {
            Secure = Request.IsHttps,
            SameSite = SameSiteMode.Lax,
            Path = "/"
        };

    private ObjectResult ToActionResult(ApplicationResult result)
    {
        if (result.Success)
            return StatusCode(result.StatusCode, new ApiResponse<object?>(true, result.Message, null, result.Meta));

        return StatusCode(result.StatusCode, new ApiErrorResponse(false, result.Message, result.Errors));
    }

    private ObjectResult ToActionResult<T>(ApplicationResult<T> result)
    {
        if (result.Success)
            return StatusCode(result.StatusCode, new ApiResponse<T>(true, result.Message, result.Data, result.Meta));

        return StatusCode(result.StatusCode, new ApiErrorResponse(false, result.Message, result.Errors));
    }
}
