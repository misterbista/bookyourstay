using backend.Features.Auth.Contracts;
using System.ComponentModel.DataAnnotations;

namespace backend.Features.Auth.Commands.Register;

public sealed record RegisterRequest(
    [property: Required, MinLength(2), MaxLength(150)] string FullName,
    [property: Required, EmailAddress, MaxLength(255)] string Email,
    [property: Required, MinLength(8), MaxLength(200)] string Password);
