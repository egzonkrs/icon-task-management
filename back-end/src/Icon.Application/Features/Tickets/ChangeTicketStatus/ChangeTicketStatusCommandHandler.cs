using FluentResults;
using Icon.Application.Abstractions;
using Icon.Domain.Common.Errors;
using Icon.Domain.Enums;
using Icon.Domain.Repositories;
using Icon.SharedKernel.Abstractions;
using MediatR;

namespace Icon.Application.Features.Tickets.ChangeTicketStatus;

/// <summary>
/// Handles transitioning a ticket to a new status.
/// </summary>
public sealed class ChangeTicketStatusCommandHandler : IRequestHandler<ChangeTicketStatusCommand, Result<bool>>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IUserContextAccessor _userContextAccessor;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeTicketStatusCommandHandler(ITicketRepository ticketRepository, IUserContextAccessor userContextAccessor, IUnitOfWork unitOfWork)
    {
        _ticketRepository = ticketRepository;
        _userContextAccessor = userContextAccessor;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(ChangeTicketStatusCommand request, CancellationToken cancellationToken)
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

        var isValidStatus = Enum.TryParse<TicketStatus>(request.Status, ignoreCase: true, out var newStatus);
        if (!isValidStatus)
        {
            return Result.Fail(TicketErrors.InvalidStatus(request.Status));
        }

        ticket.ChangeStatus(newStatus);
        _ticketRepository.Update(ticket);
        return Result.Ok(await _unitOfWork.SaveChangesAsync(cancellationToken) > 0);
    }
}
