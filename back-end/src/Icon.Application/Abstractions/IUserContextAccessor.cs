using System.Security.Claims;

namespace Icon.Application.Abstractions;

/// <summary>
/// Gives access to the currently logged-in user's identity (who is making this request).
/// </summary>
public interface IUserContextAccessor
{
    /// <summary>
    /// All claims attached to the current user's token.
    /// </summary>
    IEnumerable<Claim> Claims { get; }

    /// <summary>
    /// The current user's unique ID.
    /// </summary>
    string UserId { get; }

    /// <summary>
    /// The current user's display name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The current user's email address.
    /// </summary>
    string Email { get; }
}
