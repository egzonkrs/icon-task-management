using Icon.Domain.Entities;
using Icon.SharedKernel.Specifications;

namespace Icon.Domain.Specifications;

public sealed class TicketsByUserSpecification : Specification<Ticket>
{
    public TicketsByUserSpecification(string userId, bool? isCompleted = null, string? search = null)
    {
        Criteria = ticket => ticket.UserId == userId;

        if (isCompleted.HasValue)
        {
            Criteria = ticket => ticket.UserId == userId && ticket.IsCompleted == isCompleted.Value;
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLowerInvariant();
            Criteria = ticket => ticket.UserId == userId
                && (!isCompleted.HasValue || ticket.IsCompleted == isCompleted.Value)
                && ticket.Title.Value.Contains(term, StringComparison.InvariantCultureIgnoreCase);
        }

        ApplyOrderBy(ticket => ticket.SortOrder);
    }
}
