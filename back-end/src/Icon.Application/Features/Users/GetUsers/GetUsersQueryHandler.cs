using FluentResults;
using Icon.Application.Abstractions.Authentication;
using Icon.Application.Features.Users.Common;
using MediatR;

namespace Icon.Application.Features.Users.GetUsers;

/// <summary>
/// Handles retrieving all users.
/// </summary>
public sealed class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, Result<GetUsersResponse>>
{
    private readonly IIdentityService _identityService;

    public GetUsersQueryHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<GetUsersResponse>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _identityService.GetAllUsersAsync(cancellationToken);

        var items = users.Select(u => new UserResponse
        {
            Id = u.UserId,
            Email = u.Email,
            FullName = u.FullName
        });

        return Result.Ok(new GetUsersResponse { Items = items });
    }
}
