using Icon.Domain.Entities;

namespace Icon.Application.Features.Tickets.Common;

/// <summary>
/// Maps <see cref="Ticket"/> entities to response models.
/// </summary>
public static class TicketMapper
{
    public static TicketResponse ToResponse(Ticket ticket)
    {
        return new()
        {
            Id = ticket.Id.ToString(),
            Title = ticket.Title.Value,
            Description = ticket.Description?.Value,
            Priority = ticket.Priority.ToString(),
            Status = ticket.Status.ToString(),
            DueDate = ticket.DueDate,
            SortOrder = ticket.SortOrder,
            IsCompleted = ticket.IsCompleted,
            CompletedAt = ticket.CompletedAt,
            CreatedAt = ticket.CreatedAt,
            UpdatedAt = ticket.UpdatedAt
        };
    }
}
