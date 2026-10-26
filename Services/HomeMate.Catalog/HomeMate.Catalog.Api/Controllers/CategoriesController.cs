using HomeMate.Catalog.Application.Common.Interfaces;
using HomeMate.Catalog.Application.DTOs;
using HomeMate.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace HomeMate.Catalog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICatalogRepository _repository;

    public CategoriesController(ICatalogRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Gets all active service categories.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
    {
        var categories = await _repository.GetCategoriesAsync(includeInactive: false, cancellationToken);
        return Ok(categories);
    }

    /// <summary>
    /// Gets category details by slug including its active services.
    /// </summary>
    [HttpGet("{slug}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCategoryBySlug(string slug, CancellationToken cancellationToken)
    {
        var category = await _repository.GetCategoryBySlugAsync(slug, cancellationToken);
        if (category == null)
        {
            throw new NotFoundException($"Category with slug '{slug}' was not found.");
        }

        return Ok(new
        {
            category.Id,
            category.Name,
            category.Slug,
            category.Description,
            category.Icon,
            category.ImageUrl,
            Services = category.Services.Select(s => new ServiceItemDto(
                s.Id,
                s.CategoryId,
                category.Name,
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
        });
    }
}
