using EzyMediatr.Core.Abstractions;

namespace backend.Features.Auth.Queries.GetCurrentUser;

public sealed record GetCurrentUserRequest(string Token) : IRequest<ApplicationResult<CurrentUserResponse>>;
