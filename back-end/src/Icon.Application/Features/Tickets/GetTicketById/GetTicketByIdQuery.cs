using FluentResults;
using Icon.Application.Features.Tickets.Common;
using MediatR;

namespace Icon.Application.Features.Tickets.GetTicketById;

/// <summary>
/// Query to retrieve a single ticket by ID.
/// </summary>
public sealed record GetTicketByIdQuery : IRequest<Result<TicketDetailResponse>>
{
    public required Ulid Id { get; init; }
}
