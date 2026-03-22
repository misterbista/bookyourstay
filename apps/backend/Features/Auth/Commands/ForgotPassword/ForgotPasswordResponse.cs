namespace backend.Features.Auth.Commands.ForgotPassword;

public sealed record ForgotPasswordResponse(
    string ResetToken,
    DateTimeOffset ExpiresAt);
