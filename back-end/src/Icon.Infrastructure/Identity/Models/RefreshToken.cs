namespace Icon.Infrastructure.Identity.Models;

/// <summary>
/// A stored refresh token used to issue new access tokens without re-authentication.
/// </summary>
public sealed class RefreshToken
{
    public Guid Id { get; set; }
    public required string TokenHash { get; set; }
    public required string UserId { get; set; }
    public ApplicationUser? User { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedByIp { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? RevokedByIp { get; set; }
    public string? ReplacedByTokenHash { get; set; }
    public string? RevokedReason { get; set; }
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt.HasValue;
    public bool IsActive => !IsExpired && !IsRevoked;
}
