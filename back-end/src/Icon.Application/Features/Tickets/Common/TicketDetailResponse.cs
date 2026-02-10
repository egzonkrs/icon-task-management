namespace Icon.Application.Features.Tickets.Common;

/// <summary>
/// Detailed response model for a single ticket.
/// </summary>
public sealed record TicketDetailResponse
{
    /// <summary>The ticket ID.</summary>
    public required string Id { get; init; }

    /// <summary>The ticket title.</summary>
    public required string Title { get; init; }

    /// <summary>The ticket description.</summary>
    public string? Description { get; init; }

    /// <summary>The ticket priority.</summary>
    public required string Priority { get; init; }

    /// <summary>The ticket status.</summary>
    public required string Status { get; init; }

    /// <summary>The due date.</summary>
    public DateTime? DueDate { get; init; }

    /// <summary>The sort order for drag-and-drop positioning.</summary>
    public int SortOrder { get; init; }

    /// <summary>Whether the ticket is completed.</summary>
    public bool IsCompleted { get; init; }

    /// <summary>When the ticket was completed.</summary>
    public DateTime? CompletedAt { get; init; }

    /// <summary>When the ticket was created.</summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>When the ticket was last updated.</summary>
    public DateTime UpdatedAt { get; init; }
}
