using HomeMate.Application.Features.Authentication.DTOs;
using HomeMate.Shared.Exceptions;
using HomeMate.Auth.Application.Common.Interfaces;
using HomeMate.Auth.Application.Features.Authentication.DTOs;
using HomeMate.Auth.Application.Features.Authentication.Services;
using HomeMate.Auth.Domain.Constants;
using HomeMate.Auth.Domain.Entities;
using HomeMate.Auth.Infrastructure.Identity;
using HomeMate.Auth.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using HomeMate.Shared.Events;
using HomeMate.Shared.Messaging;

namespace HomeMate.Auth.Infrastructure.Identity.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly HomeMateDbContext _context;
    private readonly IEventBus _eventBus;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IJwtTokenGenerator jwtTokenGenerator,
        HomeMateDbContext context,
        IEventBus eventBus)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtTokenGenerator = jwtTokenGenerator;
        _context = context;
        _eventBus = eventBus;
    }

    public async Task RegisterAsync(RegisterRequest request)
    {
        var role = string.IsNullOrWhiteSpace(request.Role) ? Roles.Customer : request.Role;

        if (!Roles.All.Contains(role))
        {
            throw new BadRequestException($"Invalid role '{request.Role}'. Allowed roles: {string.Join(", ", Roles.All)}");
        }

        if (!await _roleManager.RoleExistsAsync(role))
        {
            var roleResult = await _roleManager.CreateAsync(new IdentityRole<Guid>
            {
                Id = Guid.NewGuid(),
                Name = role,
                NormalizedName = role.ToUpperInvariant()
            });

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                throw new BadRequestException($"Failed to create role '{role}': {errors}");
            }
        }

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
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

            throw new BadRequestException(errors);
        }

        await _userManager.AddToRoleAsync(user, role);

        // Publish UserRegisteredIntegrationEvent to Message Broker
        await _eventBus.PublishAsync(new UserRegisteredIntegrationEvent(
            user.Id,
            user.Email!,
            user.FirstName,
            user.LastName,
            role
        ));
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            throw new UnauthorizedException("Invalid email or password.");
        }
        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
        {
            throw new UnauthorizedException("Invalid email or password.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = _jwtTokenGenerator.GenerateToken(user.Id, user.Email!, user.FirstName, user.LastName, roles);

        var refreshToken = GenerateRefreshToken(user.Id);
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        return new AuthResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email ?? string.Empty,
            AccessToken = token,
            RefreshToken = refreshToken.Token,
            Roles = roles.ToList()
        };
    }

    public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var existingToken = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);

        if (existingToken is null || !existingToken.IsActive)
        {
            throw new BadRequestException("Invalid or expired refresh token.");
        }

        var user = await _userManager.FindByIdAsync(existingToken.UserId.ToString());

        if (user is null)
        {
            throw new NotFoundException("User", existingToken.UserId);
        }

        var roles = await _userManager.GetRolesAsync(user);

        var newAccessToken = _jwtTokenGenerator.GenerateToken(user.Id, user.Email!, user.FirstName, user.LastName, roles);
        var newRefreshToken = GenerateRefreshToken(user.Id);

        existingToken.Revoke(newRefreshToken.Token);
        _context.RefreshTokens.Add(newRefreshToken);
        await _context.SaveChangesAsync();

        return new AuthResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email ?? string.Empty,
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken.Token,
            Roles = roles.ToList()
        };
    }

    public async Task RevokeTokenAsync(string refreshToken)
    {
        var token = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshToken);
        if (token == null || !token.IsActive)
        {
            throw new BadRequestException("Invalid or expired refresh token.");
        }

        token.Revoke();
        await _context.SaveChangesAsync();
    }

    private RefreshToken GenerateRefreshToken(Guid id)
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        return new RefreshToken
        {
            UserId = id,
            Token = Convert.ToBase64String(randomBytes),
            ExpiresOnUtc = DateTime.UtcNow.AddDays(7),
            CreatedOnUtc = DateTime.UtcNow
        };

    }
}
