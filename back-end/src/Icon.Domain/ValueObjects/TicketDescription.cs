namespace Icon.Domain.ValueObjects;

public sealed record TicketDescription
{
    public const int MaxLength = 2000;
    public string Value { get; }

    private TicketDescription(string value) => Value = value;

    public static TicketDescription Create(string? value)
    {
        var sanitized = value ?? string.Empty;
        return new TicketDescription(sanitized);
    }
}
