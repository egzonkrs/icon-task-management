using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Icon.Application.Abstractions;
using Icon.Application.Abstractions.Authentication;
using Icon.Infrastructure.Data;
using Icon.Infrastructure.Identity.Configuration;
using Icon.Infrastructure.Identity.Models;
using Icon.Infrastructure.Identity.Services;
using Icon.SharedKernel.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;

namespace Icon.Api.Modules;

/// <summary>
/// Registers ASP.NET Identity, JWT Bearer authentication, and related services.
/// </summary>
public sealed class AuthenticationModule : IModule
{
    private readonly IConfiguration _configuration;

    public AuthenticationModule(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Load(IServiceCollection services)
    {
        var jwtSection = _configuration.GetSection(JwtSettings.SectionName);
        services.Configure<JwtSettings>(jwtSection);

        var jwtSettings = jwtSection.Get<JwtSettings>() ?? throw new InvalidOperationException("JWT settings are not configured.");

        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 8;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders()
        .AddSignInManager();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));
            options.MapInboundClaims = false;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = securityKey,
                ClockSkew = TimeSpan.Zero
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Cookies[jwtSettings.CookieNames.AccessToken];
                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception is SecurityTokenExpiredException)
                    {
                        context.Response.Headers.Append("X-Token-Expired", "true");
                    }
                    return Task.CompletedTask;
                },
                OnChallenge = async context =>
                {
                    context.HandleResponse();
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";

                    var errorCode = "UNAUTHORIZED_ACCESS";
                    var errorMessage = "You are not authorized to access this resource.";

                    if (context.Response.Headers.ContainsKey("X-Token-Expired"))
                    {
                        errorCode = "TOKEN_EXPIRED";
                        errorMessage = "Your session has expired. Please refresh your token or login again.";
                    }

                    await context.Response.WriteAsJsonAsync(new
                    {
                        data = (object?)null,
                        isFailed = true,
                        isSuccess = false,
                        reasons = (object?)null,
                        errors = new Dictionary<string, string> { { errorCode, errorMessage } }
                    });
                }
            };
        });

        services.AddAuthorization();
        services.AddHttpContextAccessor();

        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IJwtCookieService, JwtCookieService>();
        services.AddScoped<IUserContextAccessor, UserContextAccessor>();
        services.AddScoped<IIdentityService, IdentityService>();
    }
}
