using FluentResults;
using Icon.Application.Abstractions;
using Icon.Application.Features.Tickets.Common;
using Icon.Domain.Repositories;
using Icon.Domain.Specifications;
using MediatR;

namespace Icon.Application.Features.Tickets.GetTickets;

/// <summary>
/// Handles retrieving a list of tickets.
/// </summary>
public sealed class GetTicketsQueryHandler : IRequestHandler<GetTicketsQuery, Result<GetTicketsResponse>>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IUserContextAccessor _userContextAccessor;

    public GetTicketsQueryHandler(ITicketRepository ticketRepository, IUserContextAccessor userContextAccessor)
    {
        _ticketRepository = ticketRepository;
        _userContextAccessor = userContextAccessor;
    }

    public async Task<Result<GetTicketsResponse>> Handle(GetTicketsQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContextAccessor.UserId;
        var spec = new TicketsByUserSpecification(userId, request.IsCompleted, request.Search);
        var tickets = await _ticketRepository.ListAsync(spec, cancellationToken);

        var items = tickets.Select(TicketMapper.ToResponse);
        return Result.Ok(new GetTicketsResponse { Items = items });
    }
}
