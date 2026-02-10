using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Icon.Api.Extensions.Controller;
using Icon.Application.Features.Users.Common;
using Icon.Application.Features.Users.GetUsers;

namespace Icon.Api.Controllers;

/// <summary>
/// Controller for user-related operations.
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/users")]
[ApiVersion("1")]
[Authorize]
public sealed class UserController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// Gets all users in the system.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<GetUsersResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUsers(CancellationToken cancellationToken = default)
    {
        var query = new GetUsersQuery();
        var result = await mediator.Send(query, cancellationToken);
        return this.ToActionResult(result);
    }
}
