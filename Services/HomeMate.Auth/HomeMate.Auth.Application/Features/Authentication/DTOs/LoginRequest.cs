using System.ComponentModel.DataAnnotations;

namespace HomeMate.Auth.Application.Features.Authentication.DTOs;

public class LoginRequest
{
    [Required]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}
