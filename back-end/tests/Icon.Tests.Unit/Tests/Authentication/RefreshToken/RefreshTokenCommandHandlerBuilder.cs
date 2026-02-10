using System.Security.Claims;
using FluentResults;
using Icon.Application.Abstractions.Authentication;
using Icon.Application.Abstractions.Authentication.Models;
using Icon.Application.Features.Authentication.RefreshToken;
using Icon.Tests.Unit.Generators;
using Moq;

namespace Icon.Tests.Unit.Tests.Authentication.RefreshToken;

internal sealed class RefreshTokenCommandHandlerBuilder
{
    private readonly Mock<IIdentityService> _identityServiceMock = new();
    private readonly Mock<IJwtTokenService> _jwtTokenServiceMock = new();
    private readonly Mock<IJwtCookieService> _jwtCookieServiceMock = new();

    public RefreshTokenCommandHandlerBuilder WithGetRefreshToken(string? refreshToken)
    {
        _jwtCookieServiceMock.Setup(x => x.GetRefreshToken()).Returns(refreshToken);
        return this;
    }

    public RefreshTokenCommandHandlerBuilder WithGetAccessToken(string? accessToken)
    {
        _jwtCookieServiceMock.Setup(x => x.GetAccessToken()).Returns(accessToken);
        return this;
    }

    public RefreshTokenCommandHandlerBuilder WithValidateAccessTokenSuccess(string userId)
    {
        var identity = new ClaimsIdentity([new Claim(JwtClaimNames.UserId, userId)]);
        var principal = new ClaimsPrincipal(identity);
        _jwtTokenServiceMock
            .Setup(x => x.ValidateAccessTokenAsync(It.IsAny<string>()))
            .ReturnsAsync(Result.Ok(principal));
        return this;
    }

    public RefreshTokenCommandHandlerBuilder WithValidateAccessTokenFailed()
    {
        _jwtTokenServiceMock
            .Setup(x => x.ValidateAccessTokenAsync(It.IsAny<string>()))
            .ReturnsAsync(Result.Fail<ClaimsPrincipal>("Token expired"));
        return this;
    }

    public RefreshTokenCommandHandlerBuilder WithReadUserIdFromExpiredTokenSuccess(string userId)
    {
        _jwtTokenServiceMock
            .Setup(x => x.ReadUserIdFromExpiredToken(It.IsAny<string>()))
            .Returns(Result.Ok(userId));
        return this;
    }

    public RefreshTokenCommandHandlerBuilder WithReadUserIdFromExpiredTokenFailed(IError error)
    {
        _jwtTokenServiceMock
            .Setup(x => x.ReadUserIdFromExpiredToken(It.IsAny<string>()))
            .Returns(Result.Fail<string>(error));
        return this;
    }

    public RefreshTokenCommandHandlerBuilder WithFindByIdReturnsUser(IdentityUserModel user)
    {
        _identityServiceMock
            .Setup(x => x.FindByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        return this;
    }

    public RefreshTokenCommandHandlerBuilder WithFindByIdReturnsNull()
    {
        _identityServiceMock
            .Setup(x => x.FindByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IdentityUserModel?)null);
        return this;
    }

    public RefreshTokenCommandHandlerBuilder WithGenerateTokens(ITokenResult tokenResult)
    {
        _jwtTokenServiceMock
            .Setup(x => x.GenerateTokens(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(tokenResult);
        return this;
    }

    public RefreshTokenCommandHandlerBuilder VerifyGetRefreshTokenIsCalled(int timesCalled = 1)
    {
        _jwtCookieServiceMock.Verify(x => x.GetRefreshToken(), Times.Exactly(timesCalled));
        return this;
    }

    public RefreshTokenCommandHandlerBuilder VerifyGetAccessTokenIsCalled(int timesCalled = 1)
    {
        _jwtCookieServiceMock.Verify(x => x.GetAccessToken(), Times.Exactly(timesCalled));
        return this;
    }

    public RefreshTokenCommandHandlerBuilder VerifySetTokenCookiesIsCalled(int timesCalled = 1)
    {
        _jwtCookieServiceMock.Verify(x => x.SetTokenCookies(It.IsAny<ITokenResult>()), Times.Exactly(timesCalled));
        return this;
    }

    public RefreshTokenCommandHandlerBuilder VerifyGenerateTokensIsCalled(int timesCalled = 1)
    {
        _jwtTokenServiceMock.Verify(
            x => x.GenerateTokens(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Exactly(timesCalled));
        return this;
    }

    public RefreshTokenCommandHandlerBuilder VerifyValidateAccessTokenIsCalled(int timesCalled = 1)
    {
        _jwtTokenServiceMock.Verify(
            x => x.ValidateAccessTokenAsync(It.IsAny<string>()),
            Times.Exactly(timesCalled));
        return this;
    }

    public RefreshTokenCommandHandlerBuilder VerifyReadUserIdFromExpiredTokenIsCalled(int timesCalled = 1)
    {
        _jwtTokenServiceMock.Verify(
            x => x.ReadUserIdFromExpiredToken(It.IsAny<string>()),
            Times.Exactly(timesCalled));
        return this;
    }

    public RefreshTokenCommandHandlerBuilder VerifyFindByIdIsCalled(int timesCalled = 1)
    {
        _identityServiceMock.Verify(
            x => x.FindByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Exactly(timesCalled));
        return this;
    }

    public void VerifyNoOtherCalls()
    {
        _identityServiceMock.VerifyNoOtherCalls();
        _jwtTokenServiceMock.VerifyNoOtherCalls();
        _jwtCookieServiceMock.VerifyNoOtherCalls();
    }

    public RefreshTokenCommandHandler Build()
    {
        return new(_identityServiceMock.Object, _jwtTokenServiceMock.Object, _jwtCookieServiceMock.Object);
    }
}
