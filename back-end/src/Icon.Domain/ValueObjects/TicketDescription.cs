namespace Icon.Domain.ValueObjects;

/// <summary>
/// Value object representing a ticket description with validation.
/// </summary>
public sealed record TicketDescription
{
    public string Value { get; }

    private TicketDescription(string value) => Value = value;

    public static TicketDescription Create(string? value)
    {
        return new TicketDescription(value ?? string.Empty);
    }

    public static implicit operator string(TicketDescription description) => description.Value;
}
