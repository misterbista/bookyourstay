using backend.Features.Auth.Contracts;
using System.ComponentModel.DataAnnotations;

namespace backend.Features.Auth.Commands.Login;

public sealed record LoginRequest(
    [property: Required, EmailAddress, MaxLength(255)] string Email,
    [property: Required, MinLength(1)] string Password);
