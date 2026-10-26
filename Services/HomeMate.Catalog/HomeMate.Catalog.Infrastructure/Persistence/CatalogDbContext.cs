using HomeMate.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HomeMate.Catalog.Infrastructure.Persistence;

public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<ServiceItem> Services => Set<ServiceItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>(builder =>
        {
            builder.ToTable("Categories");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(c => c.Slug)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(c => c.Description)
                .HasMaxLength(1000);

            builder.Property(c => c.Icon)
                .HasMaxLength(100);

            builder.Property(c => c.ImageUrl)
                .HasMaxLength(1000);

            builder.HasIndex(c => c.Slug)
                .IsUnique();

            builder.HasIndex(c => c.DisplayOrder);

            builder.HasMany(c => c.Services)
                .WithOne(s => s.Category)
                .HasForeignKey(s => s.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ServiceItem>(builder =>
        {
            builder.ToTable("Services");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(s => s.Slug)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(s => s.ShortDescription)
                .HasMaxLength(300);

            builder.Property(s => s.Description)
                .HasMaxLength(4000);

            builder.Property(s => s.Price)
                .HasColumnType("decimal(18,2)");

            builder.Property(s => s.OriginalPrice)
                .HasColumnType("decimal(18,2)");

            builder.Property(s => s.ImageUrl)
                .HasMaxLength(1000);

            builder.HasIndex(s => s.CategoryId);
            builder.HasIndex(s => s.Slug);
            builder.HasIndex(s => s.IsFeatured);
            builder.HasIndex(s => s.IsActive);
        });
    }
}
