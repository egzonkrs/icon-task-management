using Icon.Application.Abstractions;
using Icon.Application.Features.Authentication.GetCurrentUser;
using Moq;

namespace Icon.Tests.Unit.Tests.Authentication.GetCurrentUser;

internal sealed class GetCurrentUserQueryHandlerBuilder
{
    private readonly Mock<IUserContextAccessor> _userContextAccessorMock = new();

    public GetCurrentUserQueryHandlerBuilder WithAuthenticatedUser(string userId, string email, string name)
    {
        _userContextAccessorMock.Setup(x => x.UserId).Returns(userId);
        _userContextAccessorMock.Setup(x => x.Email).Returns(email);
        _userContextAccessorMock.Setup(x => x.Name).Returns(name);
        return this;
    }

    public GetCurrentUserQueryHandlerBuilder WithUnauthenticatedUser()
    {
        _userContextAccessorMock.Setup(x => x.UserId).Returns(string.Empty);
        return this;
    }

    public GetCurrentUserQueryHandlerBuilder VerifyUserIdIsCalled(int timesCalled = 1)
    {
        _userContextAccessorMock.Verify(x => x.UserId, Times.Exactly(timesCalled));
        return this;
    }

    public GetCurrentUserQueryHandlerBuilder VerifyEmailIsCalled(int timesCalled = 1)
    {
        _userContextAccessorMock.Verify(x => x.Email, Times.Exactly(timesCalled));
        return this;
    }

    public GetCurrentUserQueryHandlerBuilder VerifyNameIsCalled(int timesCalled = 1)
    {
        _userContextAccessorMock.Verify(x => x.Name, Times.Exactly(timesCalled));
        return this;
    }

    public void VerifyNoOtherCalls()
    {
        _userContextAccessorMock.VerifyNoOtherCalls();
    }

    public GetCurrentUserQueryHandler Build()
    {
        return new(_userContextAccessorMock.Object);
    }
}
