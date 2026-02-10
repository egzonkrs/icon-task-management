namespace Icon.Api.Contracts.Authentication;

/// <summary>
/// Request model for user login.
/// </summary>
public sealed record LoginRequest
{
    /// <summary>
    /// The user's email address.
    /// </summary>
    public required string Email { get; init; }

    /// <summary>
    /// The user's password.
    /// </summary>
    public required string Password { get; init; }

    /// <summary>
    /// Whether to remember the user.
    /// </summary>
    public bool RememberMe { get; init; }
}
