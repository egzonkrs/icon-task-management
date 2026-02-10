using FluentResults;
using Icon.Application.Features.Users.Common;
using MediatR;

namespace Icon.Application.Features.Users.GetUsers;

/// <summary>
/// Query to get all users in the system.
/// </summary>
public sealed record GetUsersQuery : IRequest<Result<GetUsersResponse>>;
