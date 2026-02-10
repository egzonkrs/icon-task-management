using FluentValidation;

namespace Icon.Application.Features.Tickets.DeleteTicket;

public sealed class DeleteTicketCommandValidator : AbstractValidator<DeleteTicketCommand>
{
    public DeleteTicketCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Ticket ID is required.");
    }
}
