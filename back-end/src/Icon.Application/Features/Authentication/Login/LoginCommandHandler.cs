using FluentResults;
using MediatR;
using Icon.Application.Abstractions.Authentication;
using Icon.Application.Features.Authentication.Common;

namespace Icon.Application.Features.Authentication.Login;

/// <summary>
/// Handles authenticating a user with their email and password.
/// </summary>
public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthenticationResponse>>
{
    private readonly IIdentityService _identityService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IJwtCookieService _jwtCookieService;

    public LoginCommandHandler(
        IIdentityService identityService,
        IJwtTokenService jwtTokenService,
        IJwtCookieService jwtCookieService)
    {
        _identityService = identityService;
        _jwtTokenService = jwtTokenService;
        _jwtCookieService = jwtCookieService;
    }

    public async Task<Result<AuthenticationResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var signInResult = await _identityService.CheckPasswordSignInAsync(request.Email, request.Password, cancellationToken);

        if (signInResult.IsFailed)
        {
            return signInResult.ToResult<AuthenticationResponse>();
        }

        var user = signInResult.Value;
        var tokens = _jwtTokenService.GenerateTokens(user.UserId, user.Email, user.FullName);
        _jwtCookieService.SetTokenCookies(tokens);

        return Result.Ok(new AuthenticationResponse
        {
            UserId = user.UserId,
            Email = user.Email,
            FullName = user.FullName
        });
    }
}
