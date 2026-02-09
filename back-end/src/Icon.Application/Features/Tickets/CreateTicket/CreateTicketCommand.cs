using FluentResults;
using MediatR;

namespace Icon.Application.Features.Tickets.CreateTicket;

/// <summary>
/// Command to create a new ticket.
/// </summary>
public sealed record CreateTicketCommand : IRequest<Result<Ulid>>
{
    public required string Title { get; init; }
    public string? Description { get; init; }
    public string? Priority { get; init; }
    public DateTime? DueDate { get; init; }
}
