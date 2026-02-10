using Icon.SharedKernel.Common;

namespace Icon.Domain.Common.Errors;

/// <summary>
/// Domain errors related to authentication and authorization.
/// </summary>
public static class AuthenticationErrors
{
    public static CustomFluentError Unauthenticated => new("UNAUTHORIZED", "User is not authenticated.");
    public static CustomFluentError InvalidCredentials => new("INVALID_CREDENTIALS", "Invalid email or password.");
    public static CustomFluentError AccountLocked => new("ACCOUNT_LOCKED", "Account is locked. Please try again later.");
    public static CustomFluentError RefreshTokenMissing => new("INVALID_REFRESH_TOKEN", "Refresh token is missing.");
    public static CustomFluentError AccessTokenMissing => new("INVALID_ACCESS_TOKEN", "Access token is missing.");
    public static CustomFluentError AccessTokenInvalid => new("INVALID_ACCESS_TOKEN", "Invalid access token.");
    public static CustomFluentError AccessTokenInvalidFormat => new("INVALID_ACCESS_TOKEN", "Invalid access token format.");
    public static CustomFluentError EmailAlreadyExists => new("EMAIL_EXISTS", "A user with this email already exists.");
    public static CustomFluentError UserNotFound => new("USER_NOT_FOUND", "User not found.");
}
