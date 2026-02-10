using FluentAssertions;
using Icon.Application.Features.Authentication.Login;
using Icon.Domain.Common.Errors;
using Icon.SharedKernel.Common;
using Icon.Tests.Unit.Generators;

namespace Icon.Tests.Unit.Tests.Authentication.Login;

public sealed class LoginCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCredentials_ReturnsAuthenticationResponse()
    {
        // Arrange
        var user = AuthenticationGenerator.CreateIdentityUserModel();
        var tokens = AuthenticationGenerator.CreateTokenResult();

        var command = new LoginCommand
        {
            Email = user.Email,
            Password = AuthenticationGenerator.Password,
            RememberMe = false
        };

        var builder = new LoginCommandHandlerBuilder()
            .WithCheckPasswordSignInSuccess(user)
            .WithGenerateTokens(tokens);

        var handler = builder.Build();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.UserId.Should().Be(user.UserId);
        result.Value.Email.Should().Be(user.Email);
        result.Value.FullName.Should().Be(user.FullName);

        builder
            .VerifyCheckPasswordSignInIsCalled()
            .VerifyGenerateTokensIsCalled()
            .VerifySetTokenCookiesIsCalled()
            .VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_WithInvalidCredentials_ReturnsFailure()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = AuthenticationGenerator.Email,
            Password = AuthenticationGenerator.Password
        };

        var builder = new LoginCommandHandlerBuilder()
            .WithCheckPasswordSignInFailed(AuthenticationErrors.InvalidCredentials);

        var handler = builder.Build();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        CustomFluentError.GetErrorCode(result.Errors.First()).Should().Be(AuthenticationErrors.InvalidCredentials.Code);

        builder
            .VerifyCheckPasswordSignInIsCalled()
            .VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_WithLockedAccount_ReturnsFailure()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = AuthenticationGenerator.Email,
            Password = AuthenticationGenerator.Password
        };

        var builder = new LoginCommandHandlerBuilder()
            .WithCheckPasswordSignInFailed(AuthenticationErrors.AccountLocked);

        var handler = builder.Build();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        CustomFluentError.GetErrorCode(result.Errors.First()).Should().Be(AuthenticationErrors.AccountLocked.Code);

        builder
            .VerifyCheckPasswordSignInIsCalled()
            .VerifyNoOtherCalls();
    }
}
