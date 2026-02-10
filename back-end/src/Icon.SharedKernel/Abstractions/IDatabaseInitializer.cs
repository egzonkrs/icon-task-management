namespace Icon.SharedKernel.Abstractions;

/// <summary>
/// Runs on app startup to make sure the database is created and up to date (e.g. applies migrations, seeds data).
/// </summary>
public interface IDatabaseInitializer
{
    /// <summary>
    /// Creates the database (if needed) and applies any pending migrations.
    /// </summary>
    Task InitializeAsync(CancellationToken cancellationToken = default);
}
