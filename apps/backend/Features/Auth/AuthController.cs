using backend.Features.Auth.Commands.ForgotPassword;
using backend.Features.Auth.Commands.Login;
using backend.Features.Auth.Commands.Logout;
using backend.Features.Auth.Commands.Register;
using backend.Features.Auth.Commands.ResetPassword;
using backend.Features.Auth.Queries.GetCurrentUser;
using backend.Shared.Contracts;
using EzyMediatr.Core.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.Auth;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
        => ToActionResult(await mediator.Send(request, cancellationToken));

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
        => ToActionResult(await mediator.Send(request, cancellationToken));

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        if (!TryGetBearerToken(out var token, out var error))
            return error;

        return ToActionResult(await mediator.Send(new LogoutRequest(token), cancellationToken));
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
        if (!TryGetBearerToken(out var token, out var error))
            return error;

        return ToActionResult(await mediator.Send(new GetCurrentUserRequest(token), cancellationToken));
    }

    private bool TryGetBearerToken(out string token, out IActionResult error)
    {
        var value = HttpContext.GetBearerToken();
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
                ["authorization"] = ["Authorization header with bearer token is required."]
            }));
        return false;
    }

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
