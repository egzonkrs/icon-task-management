using System.Transactions;

namespace Icon.SharedKernel.Abstractions;

/// <summary>
/// Groups multiple database operations into a single save or transaction so they either all succeed or all fail.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Saves all pending changes to the database and returns how many rows were affected.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Runs the given operation inside a database transaction and returns its result.
    /// </summary>
    Task<T> ExecuteTransactionAsync<T>(Func<Task<T>> operation, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default);

    /// <summary>
    /// Runs the given operation inside a database transaction.
    /// </summary>
    Task ExecuteTransactionAsync(Func<Task> operation, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default);
}
