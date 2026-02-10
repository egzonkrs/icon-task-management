namespace Icon.Application.Features.Users.Common;

/// <summary>
/// Response containing a list of users.
/// </summary>
public sealed record GetUsersResponse
{
    public required IEnumerable<UserResponse> Items { get; init; }
}
