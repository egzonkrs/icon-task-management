using FluentResults;
using MediatR;

namespace Icon.Application.Features.Tickets.UpdateTicket;

/// <summary>
/// Command to update an existing ticket.
/// </summary>
public sealed record UpdateTicketCommand : IRequest<Result<bool>>
{
    public required Ulid Id { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public string? Priority { get; init; }
    public DateTime? DueDate { get; init; }
}
