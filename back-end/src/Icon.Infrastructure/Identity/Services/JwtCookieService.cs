using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Icon.Application.Abstractions.Authentication;
using Icon.Infrastructure.Identity.Configuration;

namespace Icon.Infrastructure.Identity.Services;

/// <summary>
/// Reads and writes JWT tokens to HTTP-only cookies.
/// </summary>
public sealed class JwtCookieService : IJwtCookieService
{
    private readonly JwtSettings _settings;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IWebHostEnvironment _environment;

    public JwtCookieService(
        IOptions<JwtSettings> settings,
        IHttpContextAccessor httpContextAccessor,
        IWebHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(httpContextAccessor);
        ArgumentNullException.ThrowIfNull(environment);

        _settings = settings.Value;
        _httpContextAccessor = httpContextAccessor;
        _environment = environment;
    }

    private HttpContext HttpContext =>
        _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("HttpContext is not available.");

    /// <inheritdoc />
    public void SetAccessTokenCookie(string token, DateTime expiration)
    {
        var options = CreateSecureCookieOptions(expiration);
        HttpContext.Response.Cookies.Append(_settings.CookieNames.AccessToken, token, options);
    }

    /// <inheritdoc />
    public void SetRefreshTokenCookie(string token, DateTime expiration)
    {
        var options = CreateSecureCookieOptions(expiration);
        options.Path = "/api/v1/auth";
        HttpContext.Response.Cookies.Append(_settings.CookieNames.RefreshToken, token, options);
    }

    /// <inheritdoc />
    public void SetTokenCookies(ITokenResult tokens)
    {
        SetAccessTokenCookie(tokens.AccessToken, tokens.AccessTokenExpiration);
        SetRefreshTokenCookie(tokens.RefreshToken, tokens.RefreshTokenExpiration);
    }

    /// <inheritdoc />
    public void ClearTokenCookies()
    {
        var accessTokenOptions = CreateSecureCookieOptions(DateTime.UtcNow.AddDays(-1));
        HttpContext.Response.Cookies.Delete(_settings.CookieNames.AccessToken, accessTokenOptions);

        var refreshTokenOptions = CreateSecureCookieOptions(DateTime.UtcNow.AddDays(-1));
        refreshTokenOptions.Path = "/api/v1/auth";
        HttpContext.Response.Cookies.Delete(_settings.CookieNames.RefreshToken, refreshTokenOptions);
    }

    /// <inheritdoc />
    public string? GetAccessToken()
    {
        return HttpContext.Request.Cookies[_settings.CookieNames.AccessToken];
    }

    /// <inheritdoc />
    public string? GetRefreshToken()
    {
        return HttpContext.Request.Cookies[_settings.CookieNames.RefreshToken];
    }

    private CookieOptions CreateSecureCookieOptions(DateTime expiration)
    {
        var isProduction = _environment.IsProduction();

        return new CookieOptions
        {
            HttpOnly = true,
            Secure = isProduction,
            SameSite = isProduction ? SameSiteMode.Strict : SameSiteMode.Lax,
            Expires = expiration,
            Path = "/",
            IsEssential = true
        };
    }
}
