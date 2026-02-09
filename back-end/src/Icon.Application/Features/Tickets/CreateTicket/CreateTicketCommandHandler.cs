using FluentResults;
using Icon.Application.Abstractions;
using Icon.Domain.Common;
using Icon.Domain.Entities;
using Icon.Domain.Enums;
using Icon.Domain.Repositories;
using Icon.Domain.ValueObjects;
using Icon.SharedKernel.Abstractions;
using MediatR;

namespace Icon.Application.Features.Tickets.CreateTicket;

/// <summary>
/// Handles creation of a new ticket.
/// </summary>
public sealed class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, Result<Ulid>>
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IUserContextAccessor _userContextAccessor;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTicketCommandHandler(
        ITicketRepository ticketRepository,
        IUserContextAccessor userContextAccessor,
        IUnitOfWork unitOfWork)
    {
        _ticketRepository = ticketRepository;
        _userContextAccessor = userContextAccessor;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Ulid>> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContextAccessor.UserId;

        var existingTicket = await _ticketRepository.GetByTitleAsync(userId, request.Title, cancellationToken);
        if (existingTicket is not null)
        {
            return Result.Fail(TicketErrors.AlreadyExists(request.Title));
        }

        var title = TicketTitle.Create(request.Title);
        var description = TicketDescription.Create(request.Description ?? string.Empty);

        var priority = TicketPriority.Medium;
        if (!string.IsNullOrWhiteSpace(request.Priority))
        {
            Enum.TryParse(request.Priority, ignoreCase: true, out priority);
        }

        var ticket = Ticket.Create(userId, title, description, priority, request.DueDate);

        await _ticketRepository.AddAsync(ticket, cancellationToken);
        var saveResult = await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (saveResult is 0)
        {
            return Result.Fail(TicketErrors.NoChangesDetected());
        }

        return Result.Ok(ticket.Id);
    }
}
