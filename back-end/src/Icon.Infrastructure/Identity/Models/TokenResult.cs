using Icon.Application.Abstractions.Authentication;

namespace Icon.Infrastructure.Identity.Models;

/// <summary>
/// Holds the generated access and refresh tokens along with their expiration times.
/// </summary>
public sealed record TokenResult : ITokenResult
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
    public required DateTime AccessTokenExpiration { get; init; }
    public required DateTime RefreshTokenExpiration { get; init; }
}
