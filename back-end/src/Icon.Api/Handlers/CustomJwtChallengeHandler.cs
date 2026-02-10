using Icon.Api.Extensions.Controller;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Icon.Api.Handlers;

/// <summary>
/// Custom handler for JWT authentication challenges.
/// Returns a consistent JSON response for 401 Unauthorized errors.
/// </summary>
public static class CustomJwtChallengeHandler
{
    /// <summary>
    /// Handles JWT authentication challenges by returning a standardized error response.
    /// </summary>
    public static async Task HandleAsync(JwtBearerChallengeContext context)
    {
        // Prevent default redirect behavior
        context.HandleResponse();

        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";

        var errorCode = "UNAUTHORIZED_ACCESS";
        var errorMessage = "You are not authorized to access this resource";

        // Check if the token was expired
        if (context.Response.Headers.ContainsKey("X-Token-Expired"))
        {
            errorCode = "TOKEN_EXPIRED";
            errorMessage = "Your session has expired. Please refresh your token or login again";
        }

        await context.Response.WriteAsJsonAsync(new ApiResponse<object>
        {
            IsFailed = true,
            IsSuccess = false,
            Errors = new Dictionary<string, string>
            {
                { errorCode, errorMessage }
            }
        });
    }
}
