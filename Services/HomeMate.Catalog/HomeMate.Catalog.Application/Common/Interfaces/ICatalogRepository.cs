using HomeMate.Catalog.Application.DTOs;
using HomeMate.Catalog.Domain.Entities;

namespace HomeMate.Catalog.Application.Common.Interfaces;

public interface ICatalogRepository
{
    Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(bool includeInactive = false, CancellationToken cancellationToken = default);
    Task<Category?> GetCategoryBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ServiceItemDto>> GetServicesAsync(Guid? categoryId = null, bool? isFeatured = null, string? searchTerm = null, CancellationToken cancellationToken = default);
    Task<ServiceItemDto?> GetServiceByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ServiceItemDto?> GetServiceBySlugAsync(string slug, CancellationToken cancellationToken = default);
}
