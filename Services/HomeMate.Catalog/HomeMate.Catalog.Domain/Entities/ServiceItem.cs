using HomeMate.Shared.Domain;

namespace HomeMate.Catalog.Domain.Entities;

public class ServiceItem : Entity<Guid>
{
    public Guid CategoryId { get; private set; }
    public Category? Category { get; private set; }

    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string ShortDescription { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public decimal? OriginalPrice { get; private set; }
    public int DurationMinutes { get; private set; }
    public string ImageUrl { get; private set; } = string.Empty;
    public double Rating { get; private set; } = 4.8;
    public int TotalReviews { get; private set; } = 0;
    public bool IsFeatured { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;

    private ServiceItem() { }

    public static ServiceItem Create(
        Guid categoryId,
        string name,
        string slug,
        string shortDescription,
        string description,
        decimal price,
        decimal? originalPrice,
        int durationMinutes,
        string imageUrl,
        bool isFeatured = false,
        double rating = 4.8,
        int totalReviews = 10)
    {
        return new ServiceItem
        {
            Id = Guid.NewGuid(),
            CategoryId = categoryId,
            Name = name,
            Slug = slug.ToLowerInvariant().Trim(),
            ShortDescription = shortDescription,
            Description = description,
            Price = price,
            OriginalPrice = originalPrice,
            DurationMinutes = durationMinutes,
            ImageUrl = imageUrl,
            IsFeatured = isFeatured,
            Rating = rating,
            TotalReviews = totalReviews,
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow
        };
    }

    public void Update(
        string name,
        string slug,
        string shortDescription,
        string description,
        decimal price,
        decimal? originalPrice,
        int durationMinutes,
        string imageUrl,
        bool isFeatured,
        bool isActive)
    {
        Name = name;
        Slug = slug.ToLowerInvariant().Trim();
        ShortDescription = shortDescription;
        Description = description;
        Price = price;
        OriginalPrice = originalPrice;
        DurationMinutes = durationMinutes;
        ImageUrl = imageUrl;
        IsFeatured = isFeatured;
        IsActive = isActive;
    }
}
