using System.Net;
using Icon.Api.Contracts.Common;
using Icon.Api.Contracts.Tickets;
using Icon.Api.Extensions.Controller;
using Icon.Application.Features.Tickets.ChangeTicketStatus;
using Icon.Application.Features.Tickets.Common;
using Icon.Application.Features.Tickets.CompleteTicket;
using Icon.Application.Features.Tickets.CreateTicket;
using Icon.Application.Features.Tickets.DeleteTicket;
using Icon.Application.Features.Tickets.GetTicketById;
using Icon.Application.Features.Tickets.GetTickets;
using Icon.Application.Features.Tickets.UpdateTicket;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Icon.Api.Controllers;

/// <summary>
/// Controller for managing tickets.
/// </summary>
[ApiController]
[Route("api/v1/tickets")]
public sealed class TicketsController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Gets all tickets for the current user.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<GetTicketsResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTickets([FromQuery] bool? isCompleted, [FromQuery] string? search, CancellationToken cancellationToken = default)
    {
        var query = new GetTicketsQuery { IsCompleted = isCompleted, Search = search };
        var result = await mediator.Send(query, cancellationToken);
        return this.ToActionResult(result);
    }

    /// <summary>
    /// Gets a specific ticket by its ID.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<TicketResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTicketById(Ulid id, CancellationToken cancellationToken = default)
    {
        var query = new GetTicketByIdQuery { Id = id };
        var result = await mediator.Send(query, cancellationToken);
        return this.ToActionResult(result);
    }

    /// <summary>
    /// Creates a new ticket.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTicket([FromBody] CreateTicketRequest request, CancellationToken cancellationToken = default)
    {
        var command = new CreateTicketCommand
        {
            Title = request.Title,
            Description = request.Description,
            Priority = request.Priority.ToString(),
            DueDate = request.DueDate
        };

        var result = await mediator.Send(command, cancellationToken);
        return this.ToActionResult(result, HttpStatusCode.Created);
    }

    /// <summary>
    /// Updates an existing ticket.
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateTicket(Ulid id, [FromBody] UpdateTicketRequest request, CancellationToken cancellationToken = default)
    {
        var command = new UpdateTicketCommand
        {
            Id = id,
            Title = request.Title,
            Description = request.Description,
            Priority = request.Priority.ToString(),
            DueDate = request.DueDate
        };

        var result = await mediator.Send(command, cancellationToken);
        return this.ToActionResult(result);
    }

    /// <summary>
    /// Changes the status of a ticket.
    /// </summary>
    [HttpPatch("{id}/status")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeTicketStatus(Ulid id, [FromBody] ChangeTicketStatusRequest request, CancellationToken cancellationToken = default)
    {
        var command = new ChangeTicketStatusCommand { Id = id, Status = request.Status.ToString() };
        var result = await mediator.Send(command, cancellationToken);
        return this.ToActionResult(result);
    }

    /// <summary>
    /// Marks a ticket as completed.
    /// </summary>
    [HttpPost("{id}/complete")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CompleteTicket(Ulid id, CancellationToken cancellationToken = default)
    {
        var command = new CompleteTicketCommand { Id = id };
        var result = await mediator.Send(command, cancellationToken);
        return this.ToActionResult(result);
    }

    /// <summary>
    /// Deletes a ticket.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTicket(Ulid id, CancellationToken cancellationToken = default)
    {
        var command = new DeleteTicketCommand { Id = id };
        var result = await mediator.Send(command, cancellationToken);
        return this.ToActionResult(result);
    }
}
