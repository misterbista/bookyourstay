using EzyMediatr.Core.Abstractions;

namespace backend.Features.Auth.Commands.Login;

public sealed record LoginRequest(
    string Email,
    string Password) : IRequest<ApplicationResult<AuthResponse>>;
