using FluentResults;
using Icon.Application.Abstractions;
using Icon.Domain.Common;
using Icon.Domain.Enums;
using Icon.Domain.Repositories;
using Icon.Domain.ValueObjects;
using Icon.SharedKernel.Abstractions;
using MediatR;

namespace Icon.Application.Features.Tickets.UpdateTicket;

/// <summary>
/// Handles updating an existing ticket.
/// </summary>
public sealed class UpdateTicketCommandHandler : IRequestHandler<UpdateTicketCommand, Result<bool>>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IUserContextAccessor _userContextAccessor;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTicketCommandHandler(
        ITicketRepository ticketRepository,
        IUserContextAccessor userContextAccessor,
        IUnitOfWork unitOfWork)
    {
        _ticketRepository = ticketRepository;
        _userContextAccessor = userContextAccessor;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(UpdateTicketCommand request, CancellationToken cancellationToken)
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

        var title = TicketTitle.Create(request.Title);
        var description = TicketDescription.Create(request.Description ?? string.Empty);
        ticket.UpdateDetails(title, description, request.DueDate);

        if (request.Priority is not null)
        {
            var isPriorityValid = Enum.TryParse<TicketPriority>(request.Priority, ignoreCase: true, out var priority);

            if (!isPriorityValid)
            {
                return Result.Fail(TicketErrors.InvalidPriority(request.Priority));
            }

            ticket.AssignPriority(priority);
        }

        _ticketRepository.Update(ticket);
        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (saveResult is 0)
        {
            return Result.Fail(TicketErrors.NoChangesDetected());
        }

        return Result.Ok(true);
    }
}
