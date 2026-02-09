using Icon.SharedKernel.Abstractions;

namespace Icon.Domain.Events;

public sealed record TicketCreatedDomainEvent(Ulid TicketId) : IDomainEvent;
