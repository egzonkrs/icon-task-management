using Icon.Domain.Enums;

namespace Icon.Api.Contracts.Tickets;

/// <summary>
/// Request model for changing ticket status.
/// </summary>
public sealed record ChangeTicketStatusRequest
{
    /// <summary>
    /// The new status to set for the ticket.
    /// </summary>
    public TicketStatus Status { get; init; }
}
