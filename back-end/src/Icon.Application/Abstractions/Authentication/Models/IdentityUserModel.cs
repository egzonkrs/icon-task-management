namespace Icon.Application.Abstractions.Authentication.Models;

/// <summary>
/// Represents a user returned from the identity system.
/// </summary>
public record IdentityUserModel(string UserId, string Email, string FullName);
