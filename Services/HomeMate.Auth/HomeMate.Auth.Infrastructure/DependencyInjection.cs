using HomeMate.Auth.Application.Features.Authentication.Services;
using HomeMate.Auth.Infrastructure.Identity;
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
        services.AddDbContext<HomeMateDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        });

        services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<HomeMateDbContext>()
            .AddDefaultTokenProviders();

        services.AddDataProtection();

        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
