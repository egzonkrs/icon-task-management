namespace Icon.Api.Contracts.Authentication;

/// <summary>
/// Request model for user registration.
/// </summary>
public sealed record RegisterRequest
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
    /// The user's first name.
    /// </summary>
    public required string FirstName { get; init; }

    /// <summary>
    /// The user's last name.
    /// </summary>
    public required string LastName { get; init; }
}
