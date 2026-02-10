using Icon.SharedKernel.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Icon.Api.Extensions.DependencyInjection;

/// <summary>
/// Extension method for registering <see cref="IModule"/> instances.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds a module to the service collection.
    /// </summary>
    public static IServiceCollection AddModule(this IServiceCollection services, IModule module)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(module);

        module.Load(services);
        return services;
    }
}
