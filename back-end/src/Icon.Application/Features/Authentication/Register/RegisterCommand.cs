using FluentResults;
using MediatR;
using Icon.Application.Features.Authentication.Common;

namespace Icon.Application.Features.Authentication.Register;

/// <summary>
/// Creates a new user account with the provided credentials and profile details.
/// </summary>
public sealed record RegisterCommand : IRequest<Result<AuthenticationResponse>>
{
    public required string Email { get; init; }
    public required string Password { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
}
