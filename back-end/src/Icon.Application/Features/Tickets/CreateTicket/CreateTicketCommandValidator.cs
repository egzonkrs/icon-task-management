using FluentValidation;

namespace Icon.Application.Features.Tickets.CreateTicket;

/// <summary>
/// Validates the <see cref="CreateTicketCommand"/>.
/// </summary>
public sealed class CreateTicketCommandValidator : AbstractValidator<CreateTicketCommand>
{
    public CreateTicketCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.");
    }
}
