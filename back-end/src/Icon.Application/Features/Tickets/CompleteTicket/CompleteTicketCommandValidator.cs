using FluentValidation;

namespace Icon.Application.Features.Tickets.CompleteTicket;

public sealed class CompleteTicketCommandValidator : AbstractValidator<CompleteTicketCommand>
{
    public CompleteTicketCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Ticket ID is required.");
    }
}
