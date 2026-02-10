using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FluentResults;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Icon.Application.Abstractions.Authentication;
using Icon.Domain.Common.Errors;
using Icon.Infrastructure.Identity.Configuration;
using Icon.Infrastructure.Identity.Models;

namespace Icon.Infrastructure.Identity.Services;

/// <summary>
/// Creates and validates JWT access tokens and refresh tokens using Microsoft.IdentityModel.
/// </summary>
public sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _settings;
    private readonly SigningCredentials _signingCredentials;
    private readonly TokenValidationParameters _validationParameters;
    private readonly JsonWebTokenHandler _tokenHandler;

    public JwtTokenService(IOptions<JwtSettings> settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        _settings = settings.Value;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
        _signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        _validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _settings.Issuer,
            ValidAudience = _settings.Audience,
            IssuerSigningKey = securityKey,
            ClockSkew = TimeSpan.Zero
        };
        _tokenHandler = new JsonWebTokenHandler();
    }

    /// <inheritdoc />
    public ITokenResult GenerateTokens(string userId, string email, string fullName)
    {
        var accessTokenExpiration = DateTime.UtcNow.AddMinutes(_settings.AccessTokenExpirationMinutes);
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_settings.RefreshTokenExpirationDays);
        var accessToken = GenerateAccessToken(userId, email, fullName, accessTokenExpiration);
        var refreshToken = GenerateRefreshTokenString();

        return new TokenResult
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiration = accessTokenExpiration,
            RefreshTokenExpiration = refreshTokenExpiration
        };
    }

    /// <inheritdoc />
    public async Task<Result<ClaimsPrincipal>> ValidateAccessTokenAsync(string token)
    {
        var result = await _tokenHandler.ValidateTokenAsync(token, _validationParameters);

        if (!result.IsValid)
        {
            return Result.Fail(AuthenticationErrors.AccessTokenInvalid);
        }

        return Result.Ok(new ClaimsPrincipal(result.ClaimsIdentity));
    }

    /// <inheritdoc />
    public Result<string> ReadUserIdFromExpiredToken(string token)
    {
        var isReadable = _tokenHandler.CanReadToken(token);
        if (!isReadable)
        {
            return Result.Fail(AuthenticationErrors.AccessTokenInvalidFormat);
        }

        var jwtToken = _tokenHandler.ReadJsonWebToken(token);
        var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Result.Fail(AuthenticationErrors.AccessTokenInvalid);
        }

        return Result.Ok(userId);
    }

    /// <inheritdoc />
    public string GenerateRefreshTokenString()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    /// <inheritdoc />
    public string HashToken(string token)
    {
        return Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
    }

    private string GenerateAccessToken(string userId, string email, string fullName, DateTime expiration)
    {
        var claims = new Dictionary<string, object>
        {
            [JwtRegisteredClaimNames.Sub] = userId,
            [JwtRegisteredClaimNames.Email] = email,
            [JwtRegisteredClaimNames.Name] = fullName,
            [JwtRegisteredClaimNames.Jti] = Guid.NewGuid().ToString()
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Issuer = _settings.Issuer,
            Audience = _settings.Audience,
            Claims = claims,
            Expires = expiration,
            IssuedAt = DateTime.UtcNow,
            NotBefore = DateTime.UtcNow,
            SigningCredentials = _signingCredentials
        };

        return _tokenHandler.CreateToken(tokenDescriptor);
    }
}
