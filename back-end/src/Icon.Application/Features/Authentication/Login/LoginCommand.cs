using FluentResults;
using MediatR;
using Icon.Application.Features.Authentication.Common;

namespace Icon.Application.Features.Authentication.Login;

/// <summary>
/// Authenticates a user with their email and password.
/// </summary>
public sealed record LoginCommand : IRequest<Result<AuthenticationResponse>>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public bool RememberMe { get; init; }
}
