using Icon.Application.Abstractions.Authentication;
using Icon.Application.Features.Authentication.Logout;
using Moq;

namespace Icon.Tests.Unit.Tests.Authentication.Logout;

internal sealed class LogoutCommandHandlerBuilder
{
    private readonly Mock<IJwtCookieService> _jwtCookieServiceMock = new();

    public LogoutCommandHandlerBuilder VerifyClearTokenCookiesIsCalled(int timesCalled = 1)
    {
        _jwtCookieServiceMock.Verify(x => x.ClearTokenCookies(), Times.Exactly(timesCalled));
        return this;
    }

    public void VerifyNoOtherCalls()
    {
        _jwtCookieServiceMock.VerifyNoOtherCalls();
    }

    public LogoutCommandHandler Build()
    {
        return new(_jwtCookieServiceMock.Object);
    }
}
