namespace Icon.Application.Features.Users.Common;

/// <summary>
/// Envelope for a list of users.
/// </summary>
public sealed record UsersResultEnvelope
{
    /// <summary>The list of users.</summary>
    public required IEnumerable<UserResponse> Items { get; init; }
}
