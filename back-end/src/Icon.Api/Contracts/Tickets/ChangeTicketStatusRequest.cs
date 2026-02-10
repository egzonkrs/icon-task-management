namespace Icon.Api.Contracts.Tickets;

/// <summary>
/// Request model for changing a ticket's status.
/// </summary>
public sealed record ChangeTicketStatusRequest
{
    /// <summary>The new status (Open, InProgress, Done, Closed).</summary>
    public required string Status { get; init; }
}
