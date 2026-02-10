using FluentAssertions;
using Icon.Application.Features.Authentication.GetCurrentUser;
using Icon.Domain.Common.Errors;
using Icon.SharedKernel.Common;
using Icon.Tests.Unit.Generators;

namespace Icon.Tests.Unit.Tests.Authentication.GetCurrentUser;

public sealed class GetCurrentUserQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithAuthenticatedUser_ReturnsUserProfile()
    {
        // Arrange
        var userId = AuthenticationGenerator.UserId;
        var email = AuthenticationGenerator.Email;
        var fullName = AuthenticationGenerator.FullName;

        var query = new GetCurrentUserQuery();

        var builder = new GetCurrentUserQueryHandlerBuilder()
            .WithAuthenticatedUser(userId, email, fullName);

        var handler = builder.Build();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.UserId.Should().Be(userId);
        result.Value.Email.Should().Be(email);
        result.Value.FullName.Should().Be(fullName);

        builder
            .VerifyUserIdIsCalled()
            .VerifyEmailIsCalled()
            .VerifyNameIsCalled()
            .VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_WithUnauthenticatedUser_ReturnsFailure()
    {
        // Arrange
        var query = new GetCurrentUserQuery();

        var builder = new GetCurrentUserQueryHandlerBuilder()
            .WithUnauthenticatedUser();

        var handler = builder.Build();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        CustomFluentError.GetErrorCode(result.Errors.First()).Should().Be(AuthenticationErrors.Unauthenticated.Code);

        builder
            .VerifyUserIdIsCalled()
            .VerifyNoOtherCalls();
    }
}
