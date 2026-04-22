using System.ComponentModel.DataAnnotations;
using backend.Features.Auth.Http;
using backend.Features.Auth.Services;
using backend.Shared.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.Auth;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController(
    AuthService auth,
    AuthCookieService authCookies) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await auth.RegisterAsync(request, cancellationToken);
        return this.ToAuthActionResult(result, authCookies);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await auth.LoginAsync(request, cancellationToken);
        return this.ToAuthActionResult(result, authCookies);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
    {
        var refreshToken = authCookies.GetRefreshToken(HttpContext);
        var result = await auth.RefreshAsync(refreshToken, cancellationToken);
        return this.ToAuthActionResult(result, authCookies);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var accessToken = authCookies.GetAccessToken(HttpContext);
        var result = await auth.LogoutAsync(accessToken, cancellationToken);
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
        var sessionClaim = User.FindFirst(JwtService.SessionIdClaim)?.Value;
        var sessionPublicId = Guid.TryParse(sessionClaim, out var parsedSessionId)
            ? parsedSessionId
            : (Guid?)null;

        var result = await auth.GetCurrentUserAsync(sessionPublicId, cancellationToken);
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
