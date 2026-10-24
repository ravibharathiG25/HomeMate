using HomeMate.Shared.Security;
using HomeMate.Shared.Web;
using Microsoft.Extensions.DependencyInjection;

namespace HomeMate.Shared;

public static class DependencyInjection
{
    /// <summary>
    /// Registers common Shared Kernel services (CurrentUserService, HttpContextAccessor, etc.).
    /// </summary>
    public static IServiceCollection AddSharedKernel(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }

    /// <summary>
    /// Registers common Web API services (GlobalExceptionHandler, ProblemDetails).
    /// </summary>
    public static IServiceCollection AddSharedWeb(this IServiceCollection services)
    {
        services.AddSharedKernel();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }
}
