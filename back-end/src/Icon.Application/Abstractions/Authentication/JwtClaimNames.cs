namespace Icon.Application.Abstractions.Authentication;

/// <summary>
/// Standard JWT claim names used across the application.
/// </summary>
public static class JwtClaimNames
{
    /// <summary>
    /// The subject claim identifying the user.
    /// </summary>
    public const string UserId = "sub";

    /// <summary>
    /// The email claim.
    /// </summary>
    public const string Email = "email";

    /// <summary>
    /// The name claim representing the user's full name.
    /// </summary>
    public const string FullName = "name";
}
