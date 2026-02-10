using FluentValidation;

namespace Icon.Application.Features.Tickets.ChangeTicketStatus;

public sealed class ChangeTicketStatusCommandValidator : AbstractValidator<ChangeTicketStatusCommand>
{
    public ChangeTicketStatusCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Ticket ID is required.");
        RuleFor(x => x.Status).NotEmpty().WithMessage("Status is required.");
    }
}
