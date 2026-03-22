using EzyMediatr.Core.Abstractions;

namespace backend.Features.Auth.Commands.Register;

public sealed record RegisterRequest(
    string FullName,
    string Email,
    string Password) : IRequest<ApplicationResult<AuthResponse>>;
