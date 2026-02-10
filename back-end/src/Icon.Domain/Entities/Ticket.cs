using Icon.Domain.Enums;
using Icon.Domain.Events;
using Icon.Domain.ValueObjects;
using Icon.SharedKernel.Abstractions;
using Icon.SharedKernel.Domain;

namespace Icon.Domain.Entities;

/// <summary>
/// Aggregate root representing a ticket in the system.
/// </summary>
public sealed class Ticket : DomainEntity, IEntity<Ulid>
{
    private Ticket(
        Ulid id,
        string userId,
        TicketTitle title,
        TicketDescription description,
        TicketPriority priority,
        TicketStatus status,
        DateTime? dueDate)
    {
        Id = id;
        UserId = userId;
        Title = title;
        Description = description;
        Priority = priority;
        Status = status;
        DueDate = dueDate;
        SortOrder = 0;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    private Ticket() { }

    public Ulid Id { get; set; }
    public string UserId { get; private set; } = null!;
    public TicketTitle Title { get; private set; } = null!;
    public TicketDescription Description { get; private set; } = null!;
    public TicketPriority Priority { get; private set; }
    public TicketStatus Status { get; private set; }
    public DateTime? DueDate { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsCompleted { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// Factory method to create a new ticket.
    /// </summary>
    public static Ticket Create(
        string userId,
        TicketTitle title,
        TicketDescription description,
        TicketPriority priority = TicketPriority.Medium,
        DateTime? dueDate = null)
    {
        var ticket = new Ticket(Ulid.NewUlid(), userId, title, description, priority, TicketStatus.Open, dueDate);
        ticket.RaiseDomainEvent(new TicketCreatedDomainEvent(ticket.Id));
        return ticket;
    }

    /// <summary>
    /// Updates mutable ticket details.
    /// </summary>
    public void UpdateDetails(TicketTitle title, TicketDescription description, DateTime? dueDate)
    {
        Title = title;
        Description = description;
        DueDate = dueDate;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Assigns a new priority level.
    /// </summary>
    public void AssignPriority(TicketPriority priority)
    {
        Priority = priority;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Transitions the ticket to a new status.
    /// </summary>
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

    /// <summary>
    /// Updates the sort order for drag-and-drop positioning.
    /// </summary>
    public void Reorder(int sortOrder)
    {
        SortOrder = sortOrder;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Marks the ticket as completed by setting status to Done.
    /// </summary>
    public void MarkAsCompleted()
    {
        if (IsCompleted)
        {
            return;
        }

        ChangeStatus(TicketStatus.Done);
    }

}
