namespace Icon.Application.Features.Authentication.Common;

/// <summary>
/// The user information returned after a successful authentication action.
/// </summary>
public sealed record AuthenticationResponse
{
    /// <summary>The user's unique identifier.</summary>
    public required string UserId { get; init; }

    /// <summary>The user's email address.</summary>
    public required string Email { get; init; }

    /// <summary>The user's full name.</summary>
    public required string FullName { get; init; }
}
