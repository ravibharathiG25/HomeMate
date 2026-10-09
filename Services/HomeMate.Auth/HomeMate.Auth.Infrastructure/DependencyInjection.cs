using HomeMate.Domain.Entities;
using HomeMate.Infrastructure.Persistance.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeMate.Infrastructure
{
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

            return services;
        }
    }
}
