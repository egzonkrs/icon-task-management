namespace Icon.Api.Contracts.Tickets;

/// <summary>
/// Request model for reordering tickets via drag-and-drop.
/// </summary>
public sealed record ReorderTicketsRequest
{
    public required IReadOnlyList<ReorderTicketItem> Items { get; init; }
}

public sealed record ReorderTicketItem
{
    public required string Id { get; init; }
    public required int SortOrder { get; init; }
}
