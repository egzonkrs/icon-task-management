using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Icon.SharedKernel.Abstractions;

namespace Icon.Infrastructure.Data;

/// <summary>
/// Initializes the database by ensuring it is created.
/// </summary>
public sealed class DatabaseInitializer : IDatabaseInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DatabaseInitializer> _logger;

    public DatabaseInitializer(ApplicationDbContext context, ILogger<DatabaseInitializer> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Database.EnsureCreatedAsync(cancellationToken);
            _logger.LogInformation("Database initialized successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initializing the database.");
            throw;
        }
    }
}
