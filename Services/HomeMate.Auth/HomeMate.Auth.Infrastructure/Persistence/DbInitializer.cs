using HomeMate.Auth.Domain.Constants;
using HomeMate.Auth.Infrastructure.Identity;
using HomeMate.Auth.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HomeMate.Auth.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger("DbInitializer");

        try
        {
            var context = services.GetRequiredService<HomeMateDbContext>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            // Automatically apply any pending migrations
            if (context.Database.IsSqlServer())
            {
                await context.Database.MigrateAsync();
            }

            // Seed predefined system roles if they don't already exist
            foreach (var role in Roles.All)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(new IdentityRole<Guid>
                    {
                        Id = Guid.NewGuid(),
                        Name = role,
                        NormalizedName = role.ToUpperInvariant()
                    });

                    if (result.Succeeded)
                    {
                        logger.LogInformation("Seeded system role: {Role}", role);
                    }
                }
            }

            // Seed default Admin user
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            const string adminEmail = "admin@homemate.com";
            var existingAdmin = await userManager.FindByEmailAsync(adminEmail);

            if (existingAdmin is null)
            {
                var adminUser = new ApplicationUser
                {
                    Id = Guid.NewGuid(),
                    FirstName = "System",
                    LastName = "Administrator",
                    Email = adminEmail,
                    UserName = adminEmail,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(adminUser, "Admin@123");
                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, Roles.Admin);
                    logger.LogInformation("Seeded default Admin user: {Email}", adminEmail);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initializing or seeding the database.");
            throw;
        }
    }
}
