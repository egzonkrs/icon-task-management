using System.Net;
using Icon.Api.Contracts.Common;
using Icon.Api.Contracts.Tickets;
using Icon.Api.Extensions.Controller;
using Icon.Application.Features.Tickets.Common;
using Icon.Application.Features.Tickets.CreateTicket;
using Icon.Application.Features.Tickets.GetTicketById;
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
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public Task<IActionResult> GetTickets(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Gets a specific ticket by its ID.
    /// </summary>
    /// <param name="id">The ticket ID.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The ticket details.</returns>
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
    /// <param name="request">The create ticket request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The created ticket ID.</returns>
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
    public Task<IActionResult> UpdateTicket(Ulid id, [FromBody] UpdateTicketRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Changes the status of a ticket.
    /// </summary>
    [HttpPatch("{id}/status")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public Task<IActionResult> ChangeTicketStatus(Ulid id, [FromBody] ChangeTicketStatusRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Marks a ticket as completed.
    /// </summary>
    [HttpPost("{id}/complete")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public Task<IActionResult> CompleteTicket(Ulid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Deletes a ticket.
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public Task<IActionResult> DeleteTicket(Ulid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
