using FluentResults;
using Icon.Application.Abstractions.Authentication;
using Icon.Application.Abstractions.Authentication.Models;
using Icon.Application.Features.Authentication.Register;
using Moq;

namespace Icon.Tests.Unit.Tests.Authentication.Register;

internal sealed class RegisterCommandHandlerBuilder
{
    private readonly Mock<IIdentityService> _identityServiceMock = new();

    public RegisterCommandHandlerBuilder WithFindByEmailReturnsNull()
    {
        _identityServiceMock
            .Setup(x => x.FindByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IdentityUserModel?)null);
        return this;
    }

    public RegisterCommandHandlerBuilder WithFindByEmailReturnsUser(IdentityUserModel user)
    {
        _identityServiceMock
            .Setup(x => x.FindByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        return this;
    }

    public RegisterCommandHandlerBuilder WithCreateUserSuccess(IdentityUserModel user)
    {
        _identityServiceMock
            .Setup(x => x.CreateUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Ok(user));
        return this;
    }

    public RegisterCommandHandlerBuilder WithCreateUserFailed(IError error)
    {
        _identityServiceMock
            .Setup(x => x.CreateUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Fail<IdentityUserModel>(error));
        return this;
    }

    public RegisterCommandHandlerBuilder VerifyFindByEmailIsCalled(int timesCalled = 1)
    {
        _identityServiceMock.Verify(
            x => x.FindByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Exactly(timesCalled));
        return this;
    }

    public RegisterCommandHandlerBuilder VerifyCreateUserIsCalled(int timesCalled = 1)
    {
        _identityServiceMock.Verify(
            x => x.CreateUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Exactly(timesCalled));
        return this;
    }

    public void VerifyNoOtherCalls()
    {
        _identityServiceMock.VerifyNoOtherCalls();
    }

    public RegisterCommandHandler Build()
    {
        return new(_identityServiceMock.Object);
    }
}
