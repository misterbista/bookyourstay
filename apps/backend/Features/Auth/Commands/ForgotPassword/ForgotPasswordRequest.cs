using EzyMediatr.Core.Abstractions;

namespace backend.Features.Auth.Commands.ForgotPassword;

public sealed record ForgotPasswordRequest(string Email) : IRequest<ApplicationResult<ForgotPasswordResponse>>;
