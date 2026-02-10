using FluentResults;
using MediatR;
using Icon.Application.Features.Authentication.Common;

namespace Icon.Application.Features.Authentication.RefreshToken;

/// <summary>
/// Refreshes the user's access token using the refresh token from the cookie.
/// </summary>
public sealed record RefreshTokenCommand : IRequest<Result<AuthenticationResponse>>;
