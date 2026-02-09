using FluentResults;
using Icon.Application.Abstractions;
using Icon.Application.Features.Tickets.Common;
using Icon.Domain.Common;
using Icon.Domain.Repositories;
using MediatR;

namespace Icon.Application.Features.Tickets.GetTicketById;

/// <summary>
/// Handles retrieving a ticket by its ID.
/// </summary>
public sealed class GetTicketByIdQueryHandler : IRequestHandler<GetTicketByIdQuery, Result<TicketResponse>>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IUserContextAccessor _userContextAccessor;

    public GetTicketByIdQueryHandler(ITicketRepository ticketRepository, IUserContextAccessor userContextAccessor)
    {
        _ticketRepository = ticketRepository;
        _userContextAccessor = userContextAccessor;
    }

    public async Task<Result<TicketResponse>> Handle(GetTicketByIdQuery request, CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetByIdAsync(request.Id, cancellationToken);
        if (ticket is null)
        {
            return Result.Fail(TicketErrors.NotFound(request.Id));
        }
        
        if (ticket.UserId != _userContextAccessor.UserId)
        {
            return Result.Fail(TicketErrors.Unauthorized());
        }

        return Result.Ok(TicketMapper.ToResponse(ticket));
    }
}
