using Icon.SharedKernel.Abstractions;

namespace Icon.Api.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddModule(this IServiceCollection services, IModule module)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(module);

        module.Load(services);
        return services;
    }
}
