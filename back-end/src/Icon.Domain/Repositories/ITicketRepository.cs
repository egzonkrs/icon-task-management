using Icon.Domain.Entities;
using Icon.SharedKernel.Abstractions;

namespace Icon.Domain.Repositories;

/// <summary>
/// Data access for tickets. Extends the generic repository with ticket-specific lookups.
/// </summary>
public interface ITicketRepository : IRepository<Ticket, Ulid>
{
    /// <summary>
    /// Finds a ticket owned by the given user that has the exact title, or null if none exists.
    /// </summary>
    Task<Ticket?> GetByTitleAsync(string userId, string title, CancellationToken cancellationToken = default);
}
