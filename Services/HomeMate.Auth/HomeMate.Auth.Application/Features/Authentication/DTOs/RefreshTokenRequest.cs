using System.ComponentModel.DataAnnotations;

namespace HomeMate.Auth.Application.Features.Authentication.DTOs;

public class RefreshTokenRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
