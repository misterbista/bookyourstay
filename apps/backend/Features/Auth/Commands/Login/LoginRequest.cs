using backend.Features.Auth.Contracts;
using System.ComponentModel.DataAnnotations;

namespace backend.Features.Auth.Commands.Login;

public sealed record LoginRequest(
    [Required, EmailAddress, MaxLength(255)] string Email,
    [Required, MinLength(1)] string Password);
