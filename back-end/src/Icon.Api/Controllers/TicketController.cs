using Asp.Versioning;
using Icon.Api.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Icon.Api.Contracts.Tickets;
using Icon.Api.Extensions.Controller;
using Icon.Application.Features.Tickets.ChangeTicketStatus;
using Icon.Application.Features.Tickets.Common;
using Icon.Application.Features.Tickets.CompleteTicket;
using Icon.Application.Features.Tickets.CreateTicket;
using Icon.Application.Features.Tickets.DeleteTicket;
using Icon.Application.Features.Tickets.GetTicketById;
using Icon.Application.Features.Tickets.GetTickets;
using Icon.Application.Features.Tickets.ReorderTickets;
using Icon.Application.Features.Tickets.UpdateTicket;
using ApplicationReorderTicketItem = Icon.Application.Features.Tickets.ReorderTickets.ReorderTicketItem;

namespace Icon.Api.Controllers;

/// <summary>
/// Controller for managing tickets.
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/tickets")]
[ApiVersion("1")]
[Authorize]
public sealed class TicketController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Gets all tickets for the current user.
    /// </summary>
    /// <param name="parameters">The query parameters for filtering.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of tickets.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<GetTicketsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetTickets([FromQuery] GetTicketsQueryParameters parameters, CancellationToken cancellationToken = default)
    {
        var query = new GetTicketsQuery
        {
            IsCompleted = parameters.IsCompleted,
            Search = parameters.Search
        };
        var result = await mediator.Send(query, cancellationToken);
        return this.ToActionResult(result);
    }

    /// <summary>
    /// Gets a specific ticket by its ID.
    /// </summary>
    /// <param name="id">The ticket ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The ticket details.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<TicketDetailResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetTicketById(Ulid id, CancellationToken cancellationToken = default)
    {
        var query = new GetTicketByIdQuery { Id = id };
        var result = await mediator.Send(query, cancellationToken);
        return this.ToActionResult(result);
    }

    /// <summary>
    /// Creates a new ticket.
    /// </summary>
    /// <param name="request">The create ticket request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created ticket ID.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateTicket([FromBody] CreateTicketRequest request, CancellationToken cancellationToken = default)
    {
        var command = new CreateTicketCommand
        {
            Title = request.Title,
            Description = request.Description,
            Priority = request.Priority,
            DueDate = request.DueDate
        };

        var result = await mediator.Send(command, cancellationToken);
        return this.ToActionResult(result, System.Net.HttpStatusCode.Created);
    }

    /// <summary>
    /// Updates an existing ticket.
    /// </summary>
    /// <param name="id">The ticket ID.</param>
    /// <param name="request">The update ticket request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Success status.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateTicket(Ulid id, [FromBody] UpdateTicketRequest request, CancellationToken cancellationToken = default)
    {
        var command = new UpdateTicketCommand
        {
            Id = id,
            Title = request.Title,
            Description = request.Description,
            Priority = request.Priority,
            DueDate = request.DueDate
        };

        var result = await mediator.Send(command, cancellationToken);
        return this.ToActionResult(result);
    }

    /// <summary>
    /// Changes the status of a ticket.
    /// </summary>
    /// <param name="id">The ticket ID.</param>
    /// <param name="request">The status change request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Success status.</returns>
    [HttpPatch("{id}/status")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangeTicketStatus(Ulid id, [FromBody] ChangeTicketStatusRequest request, CancellationToken cancellationToken = default)
    {
        var command = new ChangeTicketStatusCommand { Id = id, Status = request.Status };
        var result = await mediator.Send(command, cancellationToken);
        return this.ToActionResult(result);
    }

    /// <summary>
    /// Marks a ticket as completed.
    /// </summary>
    /// <param name="id">The ticket ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Success status.</returns>
    [HttpPost("{id}/complete")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CompleteTicket(Ulid id, CancellationToken cancellationToken = default)
    {
        var command = new CompleteTicketCommand { Id = id };
        var result = await mediator.Send(command, cancellationToken);
        return this.ToActionResult(result);
    }

    /// <summary>
    /// Reorders tickets for drag-and-drop positioning.
    /// </summary>
    /// <param name="request">The reorder request containing ticket IDs and new sort orders.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Success status.</returns>
    [HttpPost("reorder")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ReorderTickets([FromBody] ReorderTicketsRequest request, CancellationToken cancellationToken = default)
    {
        var command = new ReorderTicketsCommand
        {
            Items = request.Items.Select(item => new ApplicationReorderTicketItem
            {
                Id = Ulid.Parse(item.Id),
                SortOrder = item.SortOrder
            }).ToList()
        };

        var result = await mediator.Send(command, cancellationToken);
        return this.ToActionResult(result);
    }

    /// <summary>
    /// Deletes a ticket.
    /// </summary>
    /// <param name="id">The ticket ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Success status.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteTicket(Ulid id, CancellationToken cancellationToken = default)
    {
        var command = new DeleteTicketCommand { Id = id };
        var result = await mediator.Send(command, cancellationToken);
        return this.ToActionResult(result);
    }
}
