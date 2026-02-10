namespace Icon.Application.Abstractions.Authentication;

/// <summary>
/// Reads and writes JWT tokens to HTTP-only cookies.
/// </summary>
public interface IJwtCookieService
{
    /// <summary>
    /// Writes the access token to an HTTP-only cookie.
    /// </summary>
    void SetAccessTokenCookie(string token, DateTime expiration);

    /// <summary>
    /// Writes the refresh token to an HTTP-only cookie.
    /// </summary>
    void SetRefreshTokenCookie(string token, DateTime expiration);

    /// <summary>
    /// Writes both the access and refresh tokens to HTTP-only cookies.
    /// </summary>
    void SetTokenCookies(ITokenResult tokens);

    /// <summary>
    /// Removes the access and refresh token cookies from the response.
    /// </summary>
    void ClearTokenCookies();

    /// <summary>
    /// Reads the access token from the incoming request cookies.
    /// </summary>
    string? GetAccessToken();

    /// <summary>
    /// Reads the refresh token from the incoming request cookies.
    /// </summary>
    string? GetRefreshToken();
}
