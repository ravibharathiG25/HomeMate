using HomeMate.Auth.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HomeMate.Auth.Infrastructure.Persistence.Context;

public class HomeMateDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public HomeMateDbContext(DbContextOptions<HomeMateDbContext> options)
        : base(options)
    {
    }
}
