namespace Icon.Application.Features.Tickets.Common;

/// <summary>
/// Response model for a ticket in lists.
/// </summary>
public sealed record TicketResponse
{
    public required string Id { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public required string Priority { get; init; }
    public required string Status { get; init; }
    public DateTime? DueDate { get; init; }
    public int SortOrder { get; init; }
    public bool IsCompleted { get; init; }
    public DateTime? CompletedAt { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
