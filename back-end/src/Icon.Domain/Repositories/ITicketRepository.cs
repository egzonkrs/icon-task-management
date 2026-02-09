using Icon.Domain.Entities;
using Icon.SharedKernel.Abstractions;

namespace Icon.Domain.Repositories;

public interface ITicketRepository : IRepository<Ticket, Ulid>
{
    Task<Ticket?> GetByTitleAsync(string userId, string title, CancellationToken cancellationToken = default);
}
