using System.Diagnostics.CodeAnalysis;
using Asp.Versioning;
using Microsoft.Extensions.DependencyInjection;

namespace Icon.Api.Extensions.Web.Versioning;

/// <summary>
/// The <see cref="IServiceCollection" /> Extensions for API versioning.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Api Explorer Versioning.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to. </param>
    /// <param name="configureOptions">A delegate that is used to configure the <see cref="ApiVersioningOptions" />.</param>
    /// <returns>The same <see cref="IServiceCollection"/> so that multiple calls can be chained.</returns>
    public static IServiceCollection AddApiExplorerVersioning(
        [NotNull] this IServiceCollection services,
        Action<ApiVersioningOptions>? configureOptions = null)
    {
        ArgumentNullException.ThrowIfNull(services, nameof(services));

        services.AddApiVersioning(options =>
        {
            options.ReportApiVersions = true;
            configureOptions?.Invoke(options);
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'V";
            options.SubstituteApiVersionInUrl = true;
        });

        return services;
    }
}
