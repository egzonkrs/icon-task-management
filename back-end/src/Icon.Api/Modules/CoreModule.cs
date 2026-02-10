using FluentValidation;
using Icon.Application.Behaviors;
using Icon.Application.Features.Tickets.CreateTicket;
using Icon.SharedKernel.Abstractions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Routing;

namespace Icon.Api.Modules;

/// <summary>
/// Registers core application services: MediatR, FluentValidation, and route options.
/// </summary>
public sealed class CoreModule : IModule
{
    public void Load(IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CreateTicketCommand).Assembly);
            cfg.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(CreateTicketCommandValidator).Assembly);
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
    }
}
