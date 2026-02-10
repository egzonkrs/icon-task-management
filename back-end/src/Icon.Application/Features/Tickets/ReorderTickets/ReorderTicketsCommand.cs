using FluentResults;
using MediatR;

namespace Icon.Application.Features.Tickets.ReorderTickets;

/// <summary>
/// Command to reorder tickets for drag-and-drop positioning.
/// </summary>
public sealed record ReorderTicketsCommand : IRequest<Result<bool>>
{
    public required IReadOnlyList<ReorderTicketItem> Items { get; init; }
}

public sealed record ReorderTicketItem
{
    public required Ulid Id { get; init; }
    public required int SortOrder { get; init; }
}
