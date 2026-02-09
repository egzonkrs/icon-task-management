using Icon.Domain.Enums;

namespace Icon.Api.Contracts.Tickets;

/// <summary>
/// Request model for updating an existing ticket.
/// </summary>
public sealed record UpdateTicketRequest
{
    /// <summary>
    /// The title of the ticket.
    /// </summary>
    public required string Title { get; init; }
    
    /// <summary>
    /// The description of the ticket.
    /// </summary>
    public string? Description { get; init; }
    
    /// <summary>
    /// The priority of the ticket.
    /// </summary>
    public TicketPriority Priority { get; init; }
    
    /// <summary>
    /// The due date of the ticket.
    /// </summary>
    public DateTime? DueDate { get; init; }
}
