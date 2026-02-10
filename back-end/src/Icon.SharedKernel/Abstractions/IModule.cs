using Microsoft.Extensions.DependencyInjection;

namespace Icon.SharedKernel.Abstractions;

/// <summary>
/// Defines a modular unit of dependency injection registration.
/// </summary>
public interface IModule
{
    /// <summary>
    /// Registers services into the provided <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The service collection to register services into.</param>
    void Load(IServiceCollection services);
}
