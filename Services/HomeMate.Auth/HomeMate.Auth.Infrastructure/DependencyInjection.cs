using HomeMate.Auth.Application.Common.Interfaces;
using HomeMate.Auth.Application.Common.Models;
using HomeMate.Auth.Application.Features.Authentication.Services;
using HomeMate.Auth.Infrastructure.Identity;
using HomeMate.Auth.Infrastructure.Identity.Jwt;
using HomeMate.Auth.Infrastructure.Identity.Services;
using HomeMate.Auth.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeMate.Auth.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {

        // Bind JwtSettings from appsettings.json

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));


        // DbContext

        services.AddDbContext<HomeMateDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });

        // Identity Core

        services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<HomeMateDbContext>()
            .AddDefaultTokenProviders();

        services.AddDataProtection();

        // Application Services

        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
