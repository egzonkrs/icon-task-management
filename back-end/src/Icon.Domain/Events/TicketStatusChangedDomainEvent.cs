using Icon.Domain.Enums;
using Icon.SharedKernel.Abstractions;

namespace Icon.Domain.Events;

public sealed record TicketStatusChangedDomainEvent(
    Ulid TicketId,
    TicketStatus OldStatus,
    TicketStatus NewStatus) : IDomainEvent;
