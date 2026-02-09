using Icon.Domain.Entities;
using Icon.Domain.Repositories;
using Icon.Domain.Specifications;

namespace Icon.Infrastructure.Data.Repositories;

public sealed class TicketRepository : EfRepository<Ticket, Ulid>, ITicketRepository
{
    public TicketRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public Task<Ticket?> GetByTitleAsync(string userId, string title, CancellationToken cancellationToken = default)
    {
        var specification = new TicketByTitleSpecification(userId, title);
        return FirstOrDefaultAsync(specification, cancellationToken);
    }
}
