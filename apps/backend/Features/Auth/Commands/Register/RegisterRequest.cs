using backend.Features.Auth.Contracts;
using System.ComponentModel.DataAnnotations;

namespace backend.Features.Auth.Commands.Register;

public sealed record RegisterRequest(
    [Required, MinLength(2), MaxLength(150)] string FullName,
    [Required, EmailAddress, MaxLength(255)] string Email,
    [Required, MinLength(8), MaxLength(200)] string Password);
