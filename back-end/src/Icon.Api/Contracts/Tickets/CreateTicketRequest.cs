namespace Icon.Api.Contracts.Tickets;

/// <summary>
/// Request model for creating a new ticket.
/// </summary>
public sealed record CreateTicketRequest
{
    /// <summary>The ticket title.</summary>
    public required string Title { get; init; }

    /// <summary>The ticket description.</summary>
    public string? Description { get; init; }

    /// <summary>The ticket priority (Low, Medium, High, Critical). Defaults to Medium.</summary>
    public string? Priority { get; init; }

    /// <summary>The due date.</summary>
    public DateTime? DueDate { get; init; }
}
