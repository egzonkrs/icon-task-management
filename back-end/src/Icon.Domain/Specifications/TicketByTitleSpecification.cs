using Icon.Domain.Entities;
using Icon.SharedKernel.Specifications;

namespace Icon.Domain.Specifications;

public sealed class TicketByTitleSpecification : Specification<Ticket>
{
    public TicketByTitleSpecification(string userId, string title)
    {
        Criteria = ticket => ticket.UserId == userId && ticket.Title.Value == title;
    }
}
