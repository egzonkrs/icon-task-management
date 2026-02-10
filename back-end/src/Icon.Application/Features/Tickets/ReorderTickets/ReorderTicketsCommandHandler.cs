using FluentResults;
using Icon.Application.Abstractions;
using Icon.Domain.Common.Errors;
using Icon.Domain.Repositories;
using Icon.SharedKernel.Abstractions;
using MediatR;

namespace Icon.Application.Features.Tickets.ReorderTickets;

/// <summary>
/// Handles reordering tickets for drag-and-drop positioning.
/// </summary>
public sealed class ReorderTicketsCommandHandler : IRequestHandler<ReorderTicketsCommand, Result<bool>>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IUserContextAccessor _userContextAccessor;
    private readonly IUnitOfWork _unitOfWork;

    public ReorderTicketsCommandHandler(
        ITicketRepository ticketRepository,
        IUserContextAccessor userContextAccessor,
        IUnitOfWork unitOfWork)
    {
        _ticketRepository = ticketRepository;
        _userContextAccessor = userContextAccessor;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(ReorderTicketsCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContextAccessor.UserId;

        foreach (var item in request.Items)
        {
            var ticket = await _ticketRepository.GetByIdAsync(item.Id, cancellationToken);

            if (ticket is null)
            {
                return Result.Fail(TicketErrors.NotFound(item.Id));
            }

            if (ticket.UserId != userId)
            {
                return Result.Fail(TicketErrors.Unauthorized());
            }

            ticket.Reorder(item.SortOrder);
            _ticketRepository.Update(ticket);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(true);
    }
}
