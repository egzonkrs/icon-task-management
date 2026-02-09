using System.Security.Claims;
using Icon.Application.Abstractions;

namespace Icon.Infrastructure.Services;

/// <summary>
/// Provides access to the current user's context.
/// </summary>
public sealed class UserContextAccessor : IUserContextAccessor
{
    private const string TestUserId = "userrr";
    public IEnumerable<Claim> Claims => [];
    public string UserId => TestUserId;
    public string Name => "test user";
    public string Email => "test@example.com";
}
