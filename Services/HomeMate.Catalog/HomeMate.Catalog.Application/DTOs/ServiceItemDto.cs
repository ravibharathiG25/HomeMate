namespace HomeMate.Catalog.Application.DTOs;

public record ServiceItemDto(
    Guid Id,
    Guid CategoryId,
    string CategoryName,
    string Name,
    string Slug,
    string ShortDescription,
    string Description,
    decimal Price,
    decimal? OriginalPrice,
    int DurationMinutes,
    string ImageUrl,
    double Rating,
    int TotalReviews,
    bool IsFeatured
);
