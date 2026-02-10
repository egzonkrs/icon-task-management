using FluentResults;
using Icon.Application.Abstractions;
using Icon.Domain.Common.Errors;
using Icon.Domain.Repositories;
using Icon.SharedKernel.Abstractions;
using MediatR;

namespace Icon.Application.Features.Tickets.CompleteTicket;

/// <summary>
/// Handles marking a ticket as completed.
/// </summary>
public sealed class CompleteTicketCommandHandler : IRequestHandler<CompleteTicketCommand, Result<bool>>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IUserContextAccessor _userContextAccessor;
    private readonly IUnitOfWork _unitOfWork;

    public CompleteTicketCommandHandler(ITicketRepository ticketRepository, IUserContextAccessor userContextAccessor, IUnitOfWork unitOfWork)
    {
        _ticketRepository = ticketRepository;
        _userContextAccessor = userContextAccessor;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(CompleteTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _ticketRepository.GetByIdAsync(request.Id, cancellationToken);
        if (ticket is null) return Result.Fail(TicketErrors.NotFound(request.Id));
        if (ticket.UserId != _userContextAccessor.UserId) return Result.Fail(TicketErrors.Unauthorized());
        if (ticket.IsCompleted) return Result.Fail(TicketErrors.AlreadyCompleted());

        ticket.MarkAsCompleted();
        _ticketRepository.Update(ticket);
        return Result.Ok(await _unitOfWork.SaveChangesAsync(cancellationToken) > 0);
    }
}
