using FluentValidation;

namespace Icon.Application.Features.Tickets.ReorderTickets;

public sealed class ReorderTicketsCommandValidator : AbstractValidator<ReorderTicketsCommand>
{
    public ReorderTicketsCommandValidator()
    {
        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("At least one item is required.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.Id).NotEmpty().WithMessage("Ticket ID is required.");
            item.RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0).WithMessage("Sort order must be non-negative.");
        });
    }
}
