namespace Icon.Application.Features.Tickets.Common;

/// <summary>
/// Response model for getting a list of tickets.
/// </summary>
public sealed class GetTicketsResponse
{
    public IEnumerable<TicketResponse> Items { get; init; } = [];
}
