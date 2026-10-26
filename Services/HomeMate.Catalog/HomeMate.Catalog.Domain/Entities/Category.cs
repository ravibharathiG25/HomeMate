using HomeMate.Shared.Domain;

namespace HomeMate.Catalog.Domain.Entities;

public class Category : Entity<Guid>
{
    private readonly List<ServiceItem> _services = new();

    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Icon { get; private set; } = string.Empty;
    public string ImageUrl { get; private set; } = string.Empty;
    public int DisplayOrder { get; private set; }
    public bool IsActive { get; private set; } = true;

    public IReadOnlyCollection<ServiceItem> Services => _services.AsReadOnly();

    private Category() { }

    public static Category Create(
        string name,
        string slug,
        string description,
        string icon,
        string imageUrl,
        int displayOrder = 0)
    {
        return new Category
        {
            Id = Guid.NewGuid(),
            Name = name,
            Slug = slug.ToLowerInvariant().Trim(),
            Description = description,
            Icon = icon,
            ImageUrl = imageUrl,
            DisplayOrder = displayOrder,
            IsActive = true
        };
    }

    public void Update(string name, string slug, string description, string icon, string imageUrl, int displayOrder, bool isActive)
    {
        Name = name;
        Slug = slug.ToLowerInvariant().Trim();
        Description = description;
        Icon = icon;
        ImageUrl = imageUrl;
        DisplayOrder = displayOrder;
        IsActive = isActive;
    }

    public void AddService(ServiceItem service)
    {
        _services.Add(service);
    }
}
