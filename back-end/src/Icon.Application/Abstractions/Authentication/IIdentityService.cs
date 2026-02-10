using FluentResults;
using Icon.Application.Abstractions.Authentication.Models;

namespace Icon.Application.Abstractions.Authentication;

/// <summary>
/// Manages user sign-up, sign-in verification, and user lookups.
/// </summary>
public interface IIdentityService
{
    /// <summary>
    /// Finds a user by their email address.
    /// </summary>
    Task<IdentityUserModel?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds a user by their unique identifier.
    /// </summary>
    Task<IdentityUserModel?> FindByIdAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new user account with the given credentials and profile details.
    /// </summary>
    Task<Result<IdentityUserModel>> CreateUserAsync(string email, string password, string firstName, string lastName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Authenticates a user by email and password, returning the user on success.
    /// </summary>
    Task<Result<IdentityUserModel>> CheckPasswordSignInAsync(string email, string password, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all users in the system.
    /// </summary>
    Task<IReadOnlyList<IdentityUserModel>> GetAllUsersAsync(CancellationToken cancellationToken = default);
}
