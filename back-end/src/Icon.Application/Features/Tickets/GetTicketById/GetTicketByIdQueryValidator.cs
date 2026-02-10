using FluentValidation;

namespace Icon.Application.Features.Tickets.GetTicketById;

public sealed class GetTicketByIdQueryValidator : AbstractValidator<GetTicketByIdQuery>
{
    public GetTicketByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Ticket ID is required.");
    }
}
