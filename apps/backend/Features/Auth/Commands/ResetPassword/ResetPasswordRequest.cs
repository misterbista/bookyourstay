using EzyMediatr.Core.Abstractions;

namespace backend.Features.Auth.Commands.ResetPassword;

public sealed record ResetPasswordRequest(
    string ResetToken,
    string NewPassword) : IRequest<ApplicationResult>;
