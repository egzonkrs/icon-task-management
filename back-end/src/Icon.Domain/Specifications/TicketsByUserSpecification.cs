using Icon.Domain.Entities;
using Icon.SharedKernel.Specifications;

namespace Icon.Domain.Specifications;

public sealed class TicketsByUserSpecification : Specification<Ticket>
{
    public TicketsByUserSpecification(string userId, bool? isCompleted = null, string? search = null)
    {
        Criteria = ticket => ticket.UserId == userId
                             && IsCompletedMatch(ticket, isCompleted)
                             && SearchMatch(ticket, search);

        ApplyOrderBy(t => t.SortOrder);
    }

    private static bool IsCompletedMatch(Ticket t, bool? isCompleted)
    {
        return !isCompleted.HasValue || t.IsCompleted == isCompleted.Value;
    }

    private static bool SearchMatch(Ticket t, string? search)
    {
        return string.IsNullOrWhiteSpace(search) || t.Title.Value.Contains(search, StringComparison.CurrentCultureIgnoreCase);
    }
}
