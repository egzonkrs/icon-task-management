namespace Icon.Application.Abstractions.Authentication;

/// <summary>
/// Holds a generated access token and refresh token with their expiration times.
/// </summary>
public interface ITokenResult
{
    /// <summary>
    /// The JWT access token string.
    /// </summary>
    string AccessToken { get; }

    /// <summary>
    /// The refresh token string.
    /// </summary>
    string RefreshToken { get; }

    /// <summary>
    /// When the access token expires.
    /// </summary>
    DateTime AccessTokenExpiration { get; }

    /// <summary>
    /// When the refresh token expires.
    /// </summary>
    DateTime RefreshTokenExpiration { get; }
}
