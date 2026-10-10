using HomeMate.Application.Features.Authentication.DTOs;
using HomeMate.Auth.Application.Common.Interfaces;
using HomeMate.Auth.Application.Features.Authentication.DTOs;
using HomeMate.Auth.Application.Features.Authentication.Services;
using HomeMate.Auth.Domain.Constants;
using HomeMate.Auth.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace HomeMate.Auth.Infrastructure.Identity.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager, IJwtTokenGenerator jwtTokenGenerator)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtTokenGenerator = jwtTokenGenerator;

    }

    public async Task RegisterAsync(RegisterRequest request)
    {
        var role = string.IsNullOrWhiteSpace(request.Role) ? Roles.Customer : request.Role;

        if(!Roles.All.Contains(role))
        {
            throw new ArgumentException($"Invalid role '{request.Role}'. Allowed roles: {string.Join(", ", Roles.All)}");
        }

        if(!await _roleManager.RoleExistsAsync(role))
        {
            await _roleManager.CreateAsync(new IdentityRole<Guid> { Name = role });
        }

        var user = new ApplicationUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.Email
        };

        var result = await _userManager.CreateAsync(
            user,
            request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(
                ", ",
                result.Errors.Select(x => x.Description));

            throw new InvalidOperationException(errors);
        }

        await _userManager.AddToRoleAsync(user, role);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            throw new InvalidOperationException("Invalid email or password.");
        }
        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
        {
            throw new InvalidOperationException("Invalid email or password.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtTokenGenerator.GenerateToken(user.Id, user.Email!, user.FirstName, user.LastName, roles);
        return new AuthResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            AccessToken = token
        };
    }
}
