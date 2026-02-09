using Icon.Domain.Enums;
using Icon.Domain.Events;
using Icon.Domain.ValueObjects;
using Icon.SharedKernel.Abstractions;
using Icon.SharedKernel.Domain;

namespace Icon.Domain.Entities;

public sealed class Ticket : DomainEntity, IEntity<Ulid>
{
    public Ulid Id { get; private set; }
    public string UserId { get; private set; } = default!;
    public TicketTitle Title { get; private set; } = default!;
    public TicketDescription Description { get; private set; } = default!;
    public TicketPriority Priority { get; private set; }
    public TicketStatus Status { get; private set; }
    public DateTime? DueDate { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsCompleted { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private Ticket()
    {
    }

    public static Ticket Create(
        string userId,
        TicketTitle title,
        TicketDescription description,
        TicketPriority priority,
        DateTime? dueDate)
    {
        var ticket = new Ticket
        {
            Id = Ulid.NewUlid(),
            UserId = userId,
            Title = title,
            Description = description,
            Priority = priority,
            Status = TicketStatus.Open,
            DueDate = dueDate,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        ticket.RaiseDomainEvent(new TicketCreatedDomainEvent(ticket.Id));

        return ticket;
    }

    public void UpdateDetails(TicketTitle title, TicketDescription description, DateTime? dueDate)
    {
        Title = title;
        Description = description;
        DueDate = dueDate;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void AssignPriority(TicketPriority priority)
    {
        Priority = priority;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void ChangeStatus(TicketStatus newStatus)
    {
        var oldStatus = Status;
        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;

        var isNowCompleted = newStatus is TicketStatus.Done or TicketStatus.Closed;

        if (isNowCompleted && !IsCompleted)
        {
            IsCompleted = true;
            CompletedAt = DateTime.UtcNow;
        }
        else if (!isNowCompleted && IsCompleted)
        {
            IsCompleted = false;
            CompletedAt = null;
        }

        RaiseDomainEvent(new TicketStatusChangedDomainEvent(Id, oldStatus, newStatus));
    }
    
    public void MarkAsCompleted()
    {
        if (IsCompleted)
        {
            return;
        }

        ChangeStatus(TicketStatus.Done);
    }
}
