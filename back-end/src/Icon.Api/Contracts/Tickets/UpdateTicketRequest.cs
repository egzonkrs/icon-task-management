namespace Icon.Api.Contracts.Tickets;

/// <summary>
/// Request model for updating an existing ticket.
/// </summary>
public sealed record UpdateTicketRequest
{
    /// <summary>The new title.</summary>
    public required string Title { get; init; }

    /// <summary>The new description.</summary>
    public string? Description { get; init; }

    /// <summary>The new priority of the ticket.</summary>
    public string? Priority { get; init; }

    /// <summary>The new due date.</summary>
    public DateTime? DueDate { get; init; }
}
