using Microsoft.AspNetCore.Identity;

namespace Icon.Infrastructure.Identity.Models;

/// <summary>
/// The application's user account, extending ASP.NET Identity with profile fields.
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; init; }
}
