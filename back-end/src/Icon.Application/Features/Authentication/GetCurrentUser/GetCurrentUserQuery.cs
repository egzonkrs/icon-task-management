using FluentResults;
using MediatR;
using Icon.Application.Features.Authentication.Common;

namespace Icon.Application.Features.Authentication.GetCurrentUser;

/// <summary>
/// Retrieves the profile of the currently authenticated user.
/// </summary>
public sealed record GetCurrentUserQuery : IRequest<Result<AuthenticationResponse>>;
