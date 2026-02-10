namespace Icon.Domain.ValueObjects;

/// <summary>
/// Value object representing a ticket title with validation.
/// </summary>
public sealed record TicketTitle
{
    public string Value { get; }

    private TicketTitle(string value) => Value = value;

    public static TicketTitle Create(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));
        return new TicketTitle(value);
    }

    public static implicit operator string(TicketTitle title) => title.Value;
}
