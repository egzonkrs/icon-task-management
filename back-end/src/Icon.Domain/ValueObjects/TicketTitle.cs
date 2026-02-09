namespace Icon.Domain.ValueObjects;

public sealed record TicketTitle
{
    public string Value { get; }

    private TicketTitle(string value) => Value = value;

    public static TicketTitle Create(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));
        return new TicketTitle(value);
    }
}
