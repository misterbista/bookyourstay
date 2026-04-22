using backend.Features.Auth.Commands.Login;
using backend.Features.Auth.Commands.Logout;
using backend.Features.Auth.Commands.Refresh;
using backend.Features.Auth.Commands.Register;
using backend.Features.Auth.Contracts;
using backend.Features.Auth.Queries.GetCurrentUser;
using backend.Features.Auth.Services;
using backend.Shared.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.Auth;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController(
    RegisterCommandHandler register,
    LoginCommandHandler login,
    RefreshCommandHandler refresh,
    LogoutCommandHandler logout,
    GetCurrentUserQueryHandler getCurrentUser,
    AuthCookieService authCookies) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await register.Handle(request, cancellationToken);
        return ToResult(result, setCookies: true);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await login.Handle(request, cancellationToken);
        return ToResult(result, setCookies: true);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
    {
        var result = await refresh.Handle(new RefreshRequest(), cancellationToken);
        return ToResult(result, setCookies: true);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var result = await logout.Handle(new LogoutRequest(), cancellationToken);
        if (result.Success)
        {
            authCookies.ClearAuthCookies(HttpContext);
        }
        return ToResult(result);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        var result = await getCurrentUser.Handle(new GetCurrentUserRequest(), cancellationToken);
        return ToResult(result);
    }

    private IActionResult ToResult<T>(ApplicationResult<T> result, bool setCookies = false)
    {
        if (result.Success)
        {
            if (setCookies && result.Data is AuthResponse authResponse)
            {
                authCookies.SetAuthCookies(
                    HttpContext,
                    authResponse.AccessToken,
                    authResponse.AccessTokenExpiresAt,
                    authResponse.RefreshToken,
                    authResponse.RefreshTokenExpiresAt);
                var response = new AuthSessionResponse(authResponse.SessionId);
                return StatusCode(result.StatusCode, new ApiResponse<AuthSessionResponse>(true, result.Message, response, result.Meta));
            }

            return StatusCode(result.StatusCode, new ApiResponse<T>(true, result.Message, result.Data, result.Meta));
        }

        return StatusCode(result.StatusCode, new ApiErrorResponse(false, result.Message, result.Errors));
    }

    private IActionResult ToResult(ApplicationResult result)
    {
        if (result.Success)
        {
            return StatusCode(result.StatusCode, new ApiResponse<object?>(true, result.Message, null, result.Meta));
        }

        return StatusCode(result.StatusCode, new ApiErrorResponse(false, result.Message, result.Errors));
    }
}
