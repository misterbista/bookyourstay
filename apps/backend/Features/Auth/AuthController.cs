using System.ComponentModel.DataAnnotations;
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
        return this.ToAuthActionResult(result, authCookies);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await login.Handle(request, cancellationToken);
        return this.ToAuthActionResult(result, authCookies);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
    {
        var result = await refresh.Handle(new RefreshRequest(), cancellationToken);
        return this.ToAuthActionResult(result, authCookies);
    }

    [HttpPost("logout")]
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
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        var result = await getCurrentUser.Handle(cancellationToken);
        return this.ToActionResult(result);
    }
}

public sealed record RegisterRequest(
    [Required, MinLength(2), MaxLength(150)] string FullName,
    [Required, EmailAddress, MaxLength(255)] string Email,
    [Required, MinLength(8), MaxLength(200)] string Password);

public sealed record LoginRequest(
    [Required, EmailAddress, MaxLength(255)] string Email,
    [Required, MinLength(1)] string Password);

public sealed record RefreshRequest;

public sealed record LogoutRequest;
