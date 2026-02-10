using System.Text.Json.Serialization;

namespace Icon.Application.Features.Tickets.Common;

/// <summary>
/// Response containing a list of tickets.
/// </summary>
public sealed class GetTicketsResponse
{
    [JsonConstructor]
    public GetTicketsResponse() { }

    public IEnumerable<TicketResponse> Items { get; init; } = Array.Empty<TicketResponse>();
}
