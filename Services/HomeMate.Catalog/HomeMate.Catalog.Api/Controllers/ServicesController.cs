using HomeMate.Catalog.Application.Common.Interfaces;
using HomeMate.Catalog.Application.DTOs;
using HomeMate.Shared.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace HomeMate.Catalog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicesController : ControllerBase
{
    private readonly ICatalogRepository _repository;

    public ServicesController(ICatalogRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Gets services with optional filters (category, featured, search term).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ServiceItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetServices(
        [FromQuery] Guid? categoryId = null,
        [FromQuery] bool? featured = null,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        var services = await _repository.GetServicesAsync(categoryId, featured, search, cancellationToken);
        return Ok(services);
    }

    /// <summary>
    /// Gets top featured services for the home page.
    /// </summary>
    [HttpGet("featured")]
    [ProducesResponseType(typeof(IReadOnlyList<ServiceItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFeaturedServices(CancellationToken cancellationToken)
    {
        var services = await _repository.GetServicesAsync(categoryId: null, isFeatured: true, searchTerm: null, cancellationToken);
        return Ok(services);
    }

    /// <summary>
    /// Gets service details by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ServiceItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var service = await _repository.GetServiceByIdAsync(id, cancellationToken);
        if (service == null)
        {
            throw new NotFoundException($"Service with ID '{id}' was not found.");
        }

        return Ok(service);
    }

    /// <summary>
    /// Gets service details by Slug.
    /// </summary>
    [HttpGet("slug/{slug}")]
    [ProducesResponseType(typeof(ServiceItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBySlug(string slug, CancellationToken cancellationToken)
    {
        var service = await _repository.GetServiceBySlugAsync(slug, cancellationToken);
        if (service == null)
        {
            throw new NotFoundException($"Service with slug '{slug}' was not found.");
        }

        return Ok(service);
    }
}
