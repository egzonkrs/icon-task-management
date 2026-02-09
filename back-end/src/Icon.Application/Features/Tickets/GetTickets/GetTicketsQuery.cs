using FluentResults;
using Icon.Application.Features.Tickets.Common;
using MediatR;

namespace Icon.Application.Features.Tickets.GetTickets;

/// <summary>
/// Query to retrieve a list of tickets for the current user.
/// </summary>
public sealed record GetTicketsQuery : IRequest<Result<GetTicketsResponse>>
{
    public bool? IsCompleted { get; init; }
    public string? Search { get; init; }
}
