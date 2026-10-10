using HomeMate.Auth.Application.Features.Authentication.DTOs;

namespace HomeMate.Auth.Application.Features.Authentication.Services;

public interface IAuthService
{
    Task RegisterAsync(RegisterRequest request);
}
