namespace Icon.Infrastructure.Identity.Configuration;

/// <summary>
/// Configuration options for JWT token generation and validation.
/// </summary>
public sealed class JwtSettings
{
    public const string SectionName = "Authentication:Jwt";
    public required string SecretKey { get; init; }
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public int AccessTokenExpirationMinutes { get; init; } = 15;
    public int RefreshTokenExpirationDays { get; init; } = 7;
    public CookieNamesSettings CookieNames { get; init; } = new();
}

/// <summary>
/// The cookie names used to store access and refresh tokens.
/// </summary>
public sealed class CookieNamesSettings
{
    public string AccessToken { get; init; } = "access_token";
    public string RefreshToken { get; init; } = "refresh_token";
}
