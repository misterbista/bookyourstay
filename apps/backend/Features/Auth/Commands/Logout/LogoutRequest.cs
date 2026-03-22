using EzyMediatr.Core.Abstractions;

namespace backend.Features.Auth.Commands.Logout;

public sealed record LogoutRequest(string Token) : IRequest<ApplicationResult>;
