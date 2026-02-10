using Icon.Domain.Repositories;
using Icon.Infrastructure.Data;
using Icon.Infrastructure.Data.Repositories;
using Icon.SharedKernel.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Icon.Api.Modules;

/// <summary>
/// Registers EF Core, repositories, and the Unit of Work.
/// </summary>
public sealed class DataModule : IModule
{
    private readonly IConfiguration _configuration;

    public DataModule(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Load(IServiceCollection services)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection")
            ?? throw new ArgumentNullException("ConnectionStrings:DefaultConnection", "Cannot find 'DefaultConnection' in the configuration.");

        services.AddDbContextPool<ApplicationDbContext>(opt =>
        {
            opt.UseSqlite(connectionString);
        });

        services.TryAddScoped<IDatabaseInitializer, DatabaseInitializer>();
        services.TryAddScoped<IUnitOfWork, UnitOfWork>();
        services.TryAddScoped(typeof(IReadRepository<,>), typeof(EfRepository<,>));
        services.TryAddScoped(typeof(IRepository<,>), typeof(EfRepository<,>));
        services.TryAddScoped<ITicketRepository, TicketRepository>();
    }
}
