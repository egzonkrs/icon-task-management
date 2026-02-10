using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Icon.Api.Contracts.Authentication;
using Icon.Api.Extensions.Controller;
using Icon.Application.Features.Authentication.Common;
using Icon.Application.Features.Authentication.GetCurrentUser;
using Icon.Application.Features.Authentication.Login;
using Icon.Application.Features.Authentication.Logout;
using Icon.Application.Features.Authentication.RefreshToken;
using Icon.Application.Features.Authentication.Register;

namespace Icon.Api.Controllers;

/// <summary>
/// Controller for authentication operations.
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/auth")]
[ApiVersion("1")]
public sealed class AuthenticationController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthenticationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="request">The registration request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The registration result.</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AuthenticationResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var command = new RegisterCommand
        {
            Email = request.Email,
            Password = request.Password,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var result = await _mediator.Send(command, cancellationToken);
        return this.ToActionResult(result, System.Net.HttpStatusCode.Created);
    }

    /// <summary>
    /// Logs in a user. Sets JWT tokens in HTTP-only cookies.
    /// </summary>
    /// <param name="request">The login request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The login result.</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthenticationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken = default)
    {
        var command = new LoginCommand
        {
            Email = request.Email,
            Password = request.Password,
            RememberMe = request.RememberMe
        };

        var result = await _mediator.Send(command, cancellationToken);
        return this.ToActionResult(result);
    }

    /// <summary>
    /// Refreshes the access token using the refresh token from the cookie.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The refreshed user information.</returns>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(ApiResponse<AuthenticationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken(CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new RefreshTokenCommand(), cancellationToken);
        return this.ToActionResult(result);
    }

    /// <summary>
    /// Logs out the current user. Clears JWT cookies.
    /// </summary>
    /// <returns>The logout result.</returns>
    [HttpPost("logout")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new LogoutCommand(), cancellationToken);
        return this.ToActionResult(result);
    }

    /// <summary>
    /// Gets the current user information.
    /// </summary>
    /// <returns>The current user information.</returns>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<AuthenticationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetCurrentUserQuery(), cancellationToken);
        return this.ToActionResult(result);
    }
}
