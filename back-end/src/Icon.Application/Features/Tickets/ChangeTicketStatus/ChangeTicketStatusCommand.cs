using FluentResults;
using MediatR;

namespace Icon.Application.Features.Tickets.ChangeTicketStatus;

/// <summary>
/// Command to transition a ticket to a new status.
/// </summary>
public sealed record ChangeTicketStatusCommand : IRequest<Result<bool>>
{
    public required Ulid Id { get; init; }
    public required string Status { get; init; }
}
