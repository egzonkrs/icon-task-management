using FluentResults;
using Icon.Application.Abstractions;
using Icon.Domain.Common;
using Icon.Domain.Repositories;
using Icon.SharedKernel.Abstractions;
using MediatR;

namespace Icon.Application.Features.Tickets.DeleteTicket;

/// <summary>
/// Handles deleting a ticket.
/// </summary>
public sealed class DeleteTicketCommandHandler : IRequestHandler<DeleteTicketCommand, Result<bool>>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IUserContextAccessor _userContextAccessor;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTicketCommandHandler(ITicketRepository ticketRepository, IUserContextAccessor userContextAccessor, IUnitOfWork unitOfWork)
    {
        _ticketRepository = ticketRepository;
        _userContextAccessor = userContextAccessor;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteTicketCommand request, CancellationToken cancellationToken)
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

        _ticketRepository.Remove(ticket);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Ok(true);
    }
}
