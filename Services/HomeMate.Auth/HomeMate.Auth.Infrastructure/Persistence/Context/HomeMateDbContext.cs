using HomeMate.Auth.Domain.Entities;
using HomeMate.Auth.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace HomeMate.Auth.Infrastructure.Persistence.Context;

public class HomeMateDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public HomeMateDbContext(DbContextOptions<HomeMateDbContext> options)
        : base(options)
    {
    }

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(rt => rt.Id);
            entity.HasIndex(rt => rt.Token).IsUnique();
            entity.Property(rt => rt.Token).IsRequired().HasMaxLength(200);
        });
    }
}
