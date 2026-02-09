using System.Security.Claims;

namespace Icon.Application.Abstractions;

public interface IUserContextAccessor
{
    IEnumerable<Claim> Claims { get; }
    string UserId { get; }
    string Name { get; }
    string Email { get; }
}
