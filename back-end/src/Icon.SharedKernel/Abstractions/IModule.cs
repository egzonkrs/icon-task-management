using Microsoft.Extensions.DependencyInjection;

namespace Icon.SharedKernel.Abstractions;

public interface IModule
{
    void Load(IServiceCollection services);
}
