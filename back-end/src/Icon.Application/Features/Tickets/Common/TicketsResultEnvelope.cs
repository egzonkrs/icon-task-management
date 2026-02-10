using System.Text.Json.Serialization;

namespace Icon.Application.Features.Tickets.Common;

/// <summary>
/// Envelope wrapping ticket results.
/// </summary>
public sealed class TicketsResultEnvelope
{
    [JsonConstructor]
    public TicketsResultEnvelope() { }

    public IEnumerable<TicketResponse> Items { get; init; } = Array.Empty<TicketResponse>();
}
