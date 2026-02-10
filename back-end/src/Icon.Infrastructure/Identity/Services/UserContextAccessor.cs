using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.JsonWebTokens;
using Icon.Application.Abstractions;

namespace Icon.Infrastructure.Identity.Services;

/// <summary>
/// Provides access to the current authenticated user's claims from the HTTP context.
/// </summary>
public sealed class UserContextAccessor : IUserContextAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextAccessor(IHttpContextAccessor httpContextAccessor)
    {
        ArgumentNullException.ThrowIfNull(httpContextAccessor);

        _httpContextAccessor = httpContextAccessor;
    }

    /// <inheritdoc />
    public IEnumerable<Claim> Claims => _httpContextAccessor.HttpContext?.User?.Claims ?? [];

    /// <inheritdoc />
    public string UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? string.Empty;

    /// <inheritdoc />
    public string Name => _httpContextAccessor.HttpContext?.User?.FindFirstValue(JwtRegisteredClaimNames.Name) ?? string.Empty;

    /// <inheritdoc />
    public string Email => _httpContextAccessor.HttpContext?.User?.FindFirstValue(JwtRegisteredClaimNames.Email) ?? string.Empty;
}
