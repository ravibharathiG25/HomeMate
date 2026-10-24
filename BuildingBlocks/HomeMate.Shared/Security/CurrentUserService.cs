using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace HomeMate.Shared.Security;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Email { get; }
    string? FirstName { get; }
    string? LastName { get; }
    IReadOnlyList<string> Roles { get; }
    bool IsAuthenticated { get; }
    bool IsInRole(string role);
}

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            var idString = User?.FindFirstValue(ClaimTypes.NameIdentifier)
                           ?? User?.FindFirstValue(ClaimTypesConstants.Sub)
                           ?? User?.FindFirstValue(ClaimTypesConstants.UserId);

            return Guid.TryParse(idString, out var guid) ? guid : null;
        }
    }

    public string? Email => User?.FindFirstValue(ClaimTypes.Email)
                           ?? User?.FindFirstValue(ClaimTypesConstants.Email);

    public string? FirstName => User?.FindFirstValue(ClaimTypes.GivenName)
                               ?? User?.FindFirstValue(ClaimTypesConstants.FirstName);

    public string? LastName => User?.FindFirstValue(ClaimTypes.Surname)
                              ?? User?.FindFirstValue(ClaimTypesConstants.LastName);

    public IReadOnlyList<string> Roles => User?.FindAll(ClaimTypes.Role)
                                              .Select(c => c.Value)
                                              .ToList()
                                          ?? [];

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public bool IsInRole(string role) => User?.IsInRole(role) ?? false;
}
