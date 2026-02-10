using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Icon.Application.Abstractions.Authentication;
using Icon.Application.Abstractions.Authentication.Models;
using Icon.Domain.Common.Errors;
using Icon.SharedKernel.Common;
using Icon.Infrastructure.Identity.Models;

namespace Icon.Infrastructure.Identity.Services;

/// <summary>
/// ASP.NET Identity service implementation for user management and authentication.
/// </summary>
public sealed class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public IdentityService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    /// <inheritdoc />
    public async Task<IdentityUserModel?> FindByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user is null)
        {
            return null;
        }

        return new IdentityUserModel(user.Id, user.Email!, user.FullName);
    }

    /// <inheritdoc />
    public async Task<IdentityUserModel?> FindByIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return null;
        }

        return new IdentityUserModel(user.Id, user.Email!, user.FullName);
    }

    /// <inheritdoc />
    public async Task<Result<IdentityUserModel>> CreateUserAsync(string email, string password, string firstName, string lastName, CancellationToken cancellationToken = default)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName
        };

        var createResult = await _userManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
        {
            var errors = createResult.Errors
                .Select(e => new CustomFluentError(e.Code, e.Description))
                .Cast<IError>()
                .ToList();

            return Result.Fail(errors);
        }

        return Result.Ok(new IdentityUserModel(user.Id, user.Email!, user.FullName));
    }

    /// <inheritdoc />
    public async Task<Result<IdentityUserModel>> CheckPasswordSignInAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return Result.Fail(AuthenticationErrors.InvalidCredentials);
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: true);
        if (result.IsLockedOut)
        {
            return Result.Fail(AuthenticationErrors.AccountLocked);
        }

        if (!result.Succeeded)
        {
            return Result.Fail(AuthenticationErrors.InvalidCredentials);
        }

        return Result.Ok(new IdentityUserModel(user.Id, user.Email!, user.FullName));
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<IdentityUserModel>> GetAllUsersAsync(CancellationToken cancellationToken = default)
    {
        var users = await _userManager.Users
            .Select(u => new IdentityUserModel(u.Id, u.Email!, u.FullName))
            .ToListAsync(cancellationToken);

        return users;
    }
}
