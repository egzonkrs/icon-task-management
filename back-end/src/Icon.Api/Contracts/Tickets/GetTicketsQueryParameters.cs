using Microsoft.AspNetCore.Mvc;

namespace Icon.Api.Contracts.Tickets;

/// <summary>
/// Query parameters for getting tickets with filtering.
/// </summary>
public sealed class GetTicketsQueryParameters
{
    /// <summary>
    /// Filter by completion status. Null returns all tickets.
    /// </summary>
    [FromQuery(Name = "isCompleted")]
    public bool? IsCompleted { get; set; }

    /// <summary>
    /// Optional case-insensitive search over the title.
    /// </summary>
    [FromQuery(Name = "search")]
    public string? Search { get; set; }
}
