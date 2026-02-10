using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Icon.SharedKernel.Abstractions;

namespace Icon.Infrastructure.Data;

/// <summary>
/// Initializes the database and applies schema adjustments.
/// </summary>
public sealed class DatabaseInitializer : IDatabaseInitializer
{
    private const string TicketsTableName = "tickets";
    private const string SortOrderColumnName = "SortOrder";
    private const string EnsureSortOrderSql = "ALTER TABLE \"tickets\" ADD COLUMN \"SortOrder\" INTEGER NOT NULL DEFAULT 0;";

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
            await EnsureSortOrderColumnAsync(cancellationToken);
            _logger.LogInformation("Database initialized successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initializing the database.");
            throw;
        }
    }

    /// <summary>
    /// Ensures the SortOrder column exists for tickets.
    /// </summary>
    private async Task EnsureSortOrderColumnAsync(CancellationToken cancellationToken)
    {
        DbConnection connection = _context.Database.GetDbConnection();

        if (connection.State is not ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        bool tableExists = await TableExistsAsync(connection, cancellationToken);
        if (!tableExists)
        {
            return;
        }

        await using DbCommand command = connection.CreateCommand();
        command.CommandText = $"PRAGMA table_info('{TicketsTableName}');";

        await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        var hasSortOrder = false;

        while (await reader.ReadAsync(cancellationToken))
        {
            string columnName = reader.GetString(1);
            if (string.Equals(columnName, SortOrderColumnName, StringComparison.OrdinalIgnoreCase))
            {
                hasSortOrder = true;
                break;
            }
        }

        if (!hasSortOrder)
        {
            await _context.Database.ExecuteSqlRawAsync(EnsureSortOrderSql, cancellationToken);
            _logger.LogInformation("Added SortOrder column to tickets table.");
        }
    }

    /// <summary>
    /// Checks whether the tickets table exists.
    /// </summary>
    private static async Task<bool> TableExistsAsync(DbConnection connection, CancellationToken cancellationToken)
    {
        await using DbCommand command = connection.CreateCommand();
        command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='tickets';";
        object? result = await command.ExecuteScalarAsync(cancellationToken);
        return result is not null;
    }
}
