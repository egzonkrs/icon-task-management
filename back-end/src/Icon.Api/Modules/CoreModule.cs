using FluentValidation;
using Icon.Application.Behaviors;
using Icon.SharedKernel.Abstractions;
using MediatR;

namespace Icon.Api.Modules;

public sealed class CoreModule : IModule
{
    public void Load(IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(ValidationPipelineBehavior<,>).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(ValidationPipelineBehavior<,>).Assembly);
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
    }
}
