namespace Icon.Application.Features.Users.Common;

/// <summary>
/// Response model for a user.
/// </summary>
public sealed record UserResponse
{
    /// <summary>The user's unique identifier.</summary>
    public required string Id { get; init; }

    /// <summary>The user's email address.</summary>
    public required string Email { get; init; }

    /// <summary>The user's full name.</summary>
    public required string FullName { get; init; }
}
