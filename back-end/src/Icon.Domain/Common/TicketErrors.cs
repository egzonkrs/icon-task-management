using Icon.SharedKernel.Common;

namespace Icon.Domain.Common;

public static class TicketErrors
{
    public static CustomFluentError NotFound(Ulid ticketId) => new("TICKET_NOT_FOUND", $"Ticket with id '{ticketId}' was not found.");
    public static CustomFluentError AlreadyExists(string title) => new("TICKET_ALREADY_EXISTS", $"A ticket with the title '{title}' already exists.");
    public static CustomFluentError NoChangesDetected() => new("TICKET_NO_CHANGES", "No changes were detected.");
    public static CustomFluentError Unauthorized() => new("FORBIDDEN", "You do not have permission to access this ticket.");
    public static CustomFluentError AlreadyCompleted() => new("TICKET_ALREADY_COMPLETED", "The ticket is already completed.");
}
