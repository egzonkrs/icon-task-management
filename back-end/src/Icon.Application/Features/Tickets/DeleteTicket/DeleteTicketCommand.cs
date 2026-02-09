using FluentResults;
using MediatR;

namespace Icon.Application.Features.Tickets.DeleteTicket;

/// <summary>
/// Command to delete a ticket.
/// </summary>
public sealed record DeleteTicketCommand : IRequest<Result<bool>>
{
    public required Ulid Id { get; init; }
}
