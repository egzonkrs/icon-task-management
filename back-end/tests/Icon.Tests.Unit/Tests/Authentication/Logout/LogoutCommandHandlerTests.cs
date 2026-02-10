using FluentAssertions;
using Icon.Application.Features.Authentication.Logout;

namespace Icon.Tests.Unit.Tests.Authentication.Logout;

public sealed class LogoutCommandHandlerTests
{
    [Fact]
    public async Task Handle_ClearsCookiesAndReturnsSuccess()
    {
        // Arrange
        var command = new LogoutCommand();

        var builder = new LogoutCommandHandlerBuilder();
        var handler = builder.Build();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();

        builder
            .VerifyClearTokenCookiesIsCalled()
            .VerifyNoOtherCalls();
    }
}
