namespace HomeMate.Catalog.Application.DTOs;

public record CategoryDto(
    Guid Id,
    string Name,
    string Slug,
    string Description,
    string Icon,
    string ImageUrl,
    int DisplayOrder,
    int ServicesCount
);
