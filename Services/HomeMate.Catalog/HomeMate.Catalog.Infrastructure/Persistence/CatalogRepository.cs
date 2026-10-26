using HomeMate.Catalog.Application.Common.Interfaces;
using HomeMate.Catalog.Application.DTOs;
using HomeMate.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HomeMate.Catalog.Infrastructure.Persistence;

public class CatalogRepository : ICatalogRepository
{
    private readonly CatalogDbContext _context;

    public CatalogRepository(CatalogDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(bool includeInactive = false, CancellationToken cancellationToken = default)
    {
        var query = _context.Categories.AsNoTracking();

        if (!includeInactive)
        {
            query = query.Where(c => c.IsActive);
        }

        return await query
            .OrderBy(c => c.DisplayOrder)
            .Select(c => new CategoryDto(
                c.Id,
                c.Name,
                c.Slug,
                c.Description,
                c.Icon,
                c.ImageUrl,
                c.DisplayOrder,
                c.Services.Count(s => s.IsActive)
            ))
            .ToListAsync(cancellationToken);
    }

    public async Task<Category?> GetCategoryBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var normalized = slug.ToLowerInvariant().Trim();
        return await _context.Categories
            .AsNoTracking()
            .Include(c => c.Services.Where(s => s.IsActive))
            .FirstOrDefaultAsync(c => c.Slug == normalized, cancellationToken);
    }

    public async Task<IReadOnlyList<ServiceItemDto>> GetServicesAsync(
        Guid? categoryId = null,
        bool? isFeatured = null,
        string? searchTerm = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Services
            .AsNoTracking()
            .Include(s => s.Category)
            .Where(s => s.IsActive);

        if (categoryId.HasValue)
        {
            query = query.Where(s => s.CategoryId == categoryId.Value);
        }

        if (isFeatured.HasValue)
        {
            query = query.Where(s => s.IsFeatured == isFeatured.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(s =>
                s.Name.ToLower().Contains(term) ||
                s.ShortDescription.ToLower().Contains(term) ||
                (s.Category != null && s.Category.Name.ToLower().Contains(term)));
        }

        return await query
            .OrderByDescending(s => s.IsFeatured)
            .ThenByDescending(s => s.Rating)
            .Select(s => new ServiceItemDto(
                s.Id,
                s.CategoryId,
                s.Category != null ? s.Category.Name : string.Empty,
                s.Name,
                s.Slug,
                s.ShortDescription,
                s.Description,
                s.Price,
                s.OriginalPrice,
                s.DurationMinutes,
                s.ImageUrl,
                s.Rating,
                s.TotalReviews,
                s.IsFeatured
            ))
            .ToListAsync(cancellationToken);
    }

    public async Task<ServiceItemDto?> GetServiceByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Services
            .AsNoTracking()
            .Include(s => s.Category)
            .Where(s => s.Id == id && s.IsActive)
            .Select(s => new ServiceItemDto(
                s.Id,
                s.CategoryId,
                s.Category != null ? s.Category.Name : string.Empty,
                s.Name,
                s.Slug,
                s.ShortDescription,
                s.Description,
                s.Price,
                s.OriginalPrice,
                s.DurationMinutes,
                s.ImageUrl,
                s.Rating,
                s.TotalReviews,
                s.IsFeatured
            ))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ServiceItemDto?> GetServiceBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var normalized = slug.ToLowerInvariant().Trim();
        return await _context.Services
            .AsNoTracking()
            .Include(s => s.Category)
            .Where(s => s.Slug == normalized && s.IsActive)
            .Select(s => new ServiceItemDto(
                s.Id,
                s.CategoryId,
                s.Category != null ? s.Category.Name : string.Empty,
                s.Name,
                s.Slug,
                s.ShortDescription,
                s.Description,
                s.Price,
                s.OriginalPrice,
                s.DurationMinutes,
                s.ImageUrl,
                s.Rating,
                s.TotalReviews,
                s.IsFeatured
            ))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
