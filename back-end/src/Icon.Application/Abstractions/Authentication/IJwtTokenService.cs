using System.Security.Claims;
using FluentResults;

namespace Icon.Application.Abstractions.Authentication;

/// <summary>
/// Creates and validates JWT access tokens and refresh tokens.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Generates a new access token and refresh token pair for the given user.
    /// </summary>
    ITokenResult GenerateTokens(string userId, string email, string fullName);

    /// <summary>
    /// Validates an access token and returns the claims principal if valid.
    /// </summary>
    Task<Result<ClaimsPrincipal>> ValidateAccessTokenAsync(string token);

    /// <summary>
    /// Reads the user ID from an expired access token without validating its lifetime.
    /// </summary>
    Result<string> ReadUserIdFromExpiredToken(string token);

    /// <summary>
    /// Generates a cryptographically secure random refresh token string.
    /// </summary>
    string GenerateRefreshTokenString();

    /// <summary>
    /// Produces a SHA-256 hash of the given token for secure storage.
    /// </summary>
    string HashToken(string token);
}
