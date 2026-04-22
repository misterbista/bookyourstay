using backend.Features.Auth.Commands.Login;
using backend.Features.Auth.Commands.Logout;
using backend.Features.Auth.Commands.Refresh;
using backend.Features.Auth.Commands.Register;
using backend.Features.Auth.Http;
using backend.Features.Auth.Queries.GetCurrentUser;
using backend.Features.Auth.Services;
using backend.Shared.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.Auth;

[ApiController]
[Route(AuthRoutes.Base)]
public sealed class AuthController(
    RegisterCommandHandler register,
    LoginCommandHandler login,
    RefreshCommandHandler refresh,
    LogoutCommandHandler logout,
    GetCurrentUserQueryHandler getCurrentUser,
    AuthCookieService authCookies) : ControllerBase
{
    [HttpPost(AuthRoutes.Register)]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await register.Handle(request, cancellationToken);
        return this.ToAuthActionResult(result, authCookies);
    }

    [HttpPost(AuthRoutes.Login)]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await login.Handle(request, cancellationToken);
        return this.ToAuthActionResult(result, authCookies);
    }

    [HttpPost(AuthRoutes.Refresh)]
    public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
    {
        var result = await refresh.Handle(new RefreshRequest(), cancellationToken);
        return this.ToAuthActionResult(result, authCookies);
    }

    [HttpPost(AuthRoutes.Logout)]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var result = await logout.Handle(new LogoutRequest(), cancellationToken);
        if (result.Success)
        {
            authCookies.ClearAuthCookies(HttpContext);
        }
        return this.ToActionResult(result);
    }

    [Authorize]
    [HttpGet(AuthRoutes.CurrentUser)]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        var result = await getCurrentUser.Handle(new GetCurrentUserRequest(), cancellationToken);
        return this.ToActionResult(result);
    }
}
