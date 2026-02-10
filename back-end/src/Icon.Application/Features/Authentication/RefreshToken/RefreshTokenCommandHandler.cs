using FluentResults;
using MediatR;
using Icon.Application.Abstractions.Authentication;
using Icon.Application.Features.Authentication.Common;
using Icon.Domain.Common.Errors;

namespace Icon.Application.Features.Authentication.RefreshToken;

/// <summary>
/// Handles refreshing the user's access token using the refresh token from the cookie.
/// </summary>
public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthenticationResponse>>
{
    private readonly IIdentityService _identityService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IJwtCookieService _jwtCookieService;

    public RefreshTokenCommandHandler(
        IIdentityService identityService,
        IJwtTokenService jwtTokenService,
        IJwtCookieService jwtCookieService)
    {
        _identityService = identityService;
        _jwtTokenService = jwtTokenService;
        _jwtCookieService = jwtCookieService;
    }

    public async Task<Result<AuthenticationResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var refreshToken = _jwtCookieService.GetRefreshToken();
        if (string.IsNullOrEmpty(refreshToken))
        {
            return Result.Fail(AuthenticationErrors.RefreshTokenMissing);
        }

        var expiredAccessToken = _jwtCookieService.GetAccessToken();
        if (string.IsNullOrEmpty(expiredAccessToken))
        {
            return Result.Fail(AuthenticationErrors.AccessTokenMissing);
        }

        var userIdResult = await ExtractUserIdAsync(expiredAccessToken);
        if (userIdResult.IsFailed)
        {
            return userIdResult.ToResult<AuthenticationResponse>();
        }

        var userId = userIdResult.Value;
        var user = await _identityService.FindByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return Result.Fail(AuthenticationErrors.UserNotFound);
        }

        var tokens = _jwtTokenService.GenerateTokens(user.UserId, user.Email, user.FullName);
        _jwtCookieService.SetTokenCookies(tokens);

        return Result.Ok(new AuthenticationResponse
        {
            UserId = user.UserId,
            Email = user.Email,
            FullName = user.FullName
        });
    }

    private async Task<Result<string>> ExtractUserIdAsync(string accessToken)
    {
        var validationResult = await _jwtTokenService.ValidateAccessTokenAsync(accessToken);

        if (validationResult.IsSuccess)
        {
            var principal = validationResult.Value;
            var userId = principal.FindFirst(JwtClaimNames.UserId)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Result.Fail(AuthenticationErrors.AccessTokenInvalid);
            }

            return Result.Ok(userId);
        }

        return _jwtTokenService.ReadUserIdFromExpiredToken(accessToken);
    }
}
