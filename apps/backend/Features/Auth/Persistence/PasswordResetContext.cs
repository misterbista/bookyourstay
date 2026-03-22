using backend.Features.Auth.Domain;

namespace backend.Features.Auth.Persistence;

public sealed record PasswordResetContext(
    AuthUser User,
    AuthIdentity Identity,
    PasswordResetTicket Ticket);
