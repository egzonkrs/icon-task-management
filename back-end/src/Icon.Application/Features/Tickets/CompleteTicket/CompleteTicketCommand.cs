using FluentResults;
using MediatR;

namespace Icon.Application.Features.Tickets.CompleteTicket;

/// <summary>
/// Command to mark a ticket as completed.
/// </summary>
public sealed record CompleteTicketCommand : IRequest<Result<bool>>
{
    public required Ulid Id { get; init; }
}
