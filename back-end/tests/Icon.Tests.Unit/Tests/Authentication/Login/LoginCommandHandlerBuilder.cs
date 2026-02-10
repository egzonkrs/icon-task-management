using FluentResults;
using Icon.Application.Abstractions.Authentication;
using Icon.Application.Abstractions.Authentication.Models;
using Icon.Application.Features.Authentication.Login;
using Icon.Tests.Unit.Generators;
using Moq;

namespace Icon.Tests.Unit.Tests.Authentication.Login;

internal sealed class LoginCommandHandlerBuilder
{
    private readonly Mock<IIdentityService> _identityServiceMock = new();
    private readonly Mock<IJwtTokenService> _jwtTokenServiceMock = new();
    private readonly Mock<IJwtCookieService> _jwtCookieServiceMock = new();

    public LoginCommandHandlerBuilder WithCheckPasswordSignInSuccess(IdentityUserModel user)
    {
        _identityServiceMock
            .Setup(x => x.CheckPasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(user));
        return this;
    }

    public LoginCommandHandlerBuilder WithCheckPasswordSignInFailed(IError error)
    {
        _identityServiceMock
            .Setup(x => x.CheckPasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail<IdentityUserModel>(error));
        return this;
    }

    public LoginCommandHandlerBuilder WithGenerateTokens(ITokenResult tokenResult)
    {
        _jwtTokenServiceMock
            .Setup(x => x.GenerateTokens(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(tokenResult);
        return this;
    }

    public LoginCommandHandlerBuilder VerifyCheckPasswordSignInIsCalled(int timesCalled = 1)
    {
        _identityServiceMock.Verify(
            x => x.CheckPasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Exactly(timesCalled));
        return this;
    }

    public LoginCommandHandlerBuilder VerifyGenerateTokensIsCalled(int timesCalled = 1)
    {
        _jwtTokenServiceMock.Verify(
            x => x.GenerateTokens(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
            Times.Exactly(timesCalled));
        return this;
    }

    public LoginCommandHandlerBuilder VerifySetTokenCookiesIsCalled(int timesCalled = 1)
    {
        _jwtCookieServiceMock.Verify(
            x => x.SetTokenCookies(It.IsAny<ITokenResult>()),
            Times.Exactly(timesCalled));
        return this;
    }

    public void VerifyNoOtherCalls()
    {
        _identityServiceMock.VerifyNoOtherCalls();
        _jwtTokenServiceMock.VerifyNoOtherCalls();
        _jwtCookieServiceMock.VerifyNoOtherCalls();
    }

    public LoginCommandHandler Build()
    {
        return new(_identityServiceMock.Object, _jwtTokenServiceMock.Object, _jwtCookieServiceMock.Object);
    }
}
