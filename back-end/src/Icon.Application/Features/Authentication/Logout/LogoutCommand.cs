using FluentResults;
using MediatR;

namespace Icon.Application.Features.Authentication.Logout;

/// <summary>
/// Logs the current user out by clearing their token cookies.
/// </summary>
public sealed record LogoutCommand : IRequest<Result<bool>>;
