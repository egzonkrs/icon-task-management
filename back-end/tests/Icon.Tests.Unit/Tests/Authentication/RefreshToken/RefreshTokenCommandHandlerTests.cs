using FluentAssertions;
using Icon.Application.Features.Authentication.RefreshToken;
using Icon.Domain.Common.Errors;
using Icon.SharedKernel.Common;
using Icon.Tests.Unit.Generators;

namespace Icon.Tests.Unit.Tests.Authentication.RefreshToken;

public sealed class RefreshTokenCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidTokens_ReturnsAuthenticationResponse()
    {
        // Arrange
        var user = AuthenticationGenerator.CreateIdentityUserModel();
        var tokens = AuthenticationGenerator.CreateTokenResult();
        var command = new RefreshTokenCommand();

        var builder = new RefreshTokenCommandHandlerBuilder()
            .WithGetRefreshToken("valid-refresh-token")
            .WithGetAccessToken("valid-access-token")
            .WithValidateAccessTokenSuccess(user.UserId)
            .WithFindByIdReturnsUser(user)
            .WithGenerateTokens(tokens);

        var handler = builder.Build();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.UserId.Should().Be(user.UserId);
        result.Value.Email.Should().Be(user.Email);

        builder
            .VerifyGetRefreshTokenIsCalled()
            .VerifyGetAccessTokenIsCalled()
            .VerifyValidateAccessTokenIsCalled()
            .VerifyFindByIdIsCalled()
            .VerifyGenerateTokensIsCalled()
            .VerifySetTokenCookiesIsCalled()
            .VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_WhenRefreshTokenMissing_ReturnsFailure()
    {
        // Arrange
        var command = new RefreshTokenCommand();

        var builder = new RefreshTokenCommandHandlerBuilder()
            .WithGetRefreshToken(null);

        var handler = builder.Build();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        CustomFluentError.GetErrorCode(result.Errors.First()).Should().Be(AuthenticationErrors.RefreshTokenMissing.Code);

        builder
            .VerifyGetRefreshTokenIsCalled()
            .VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_WhenAccessTokenMissing_ReturnsFailure()
    {
        // Arrange
        var command = new RefreshTokenCommand();

        var builder = new RefreshTokenCommandHandlerBuilder()
            .WithGetRefreshToken("valid-refresh-token")
            .WithGetAccessToken(null);

        var handler = builder.Build();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        CustomFluentError.GetErrorCode(result.Errors.First()).Should().Be(AuthenticationErrors.AccessTokenMissing.Code);

        builder
            .VerifyGetRefreshTokenIsCalled()
            .VerifyGetAccessTokenIsCalled()
            .VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_WhenAccessTokenExpiredButReadable_ReturnsSuccess()
    {
        // Arrange
        var user = AuthenticationGenerator.CreateIdentityUserModel();
        var tokens = AuthenticationGenerator.CreateTokenResult();
        var command = new RefreshTokenCommand();

        var builder = new RefreshTokenCommandHandlerBuilder()
            .WithGetRefreshToken("valid-refresh-token")
            .WithGetAccessToken("expired-access-token")
            .WithValidateAccessTokenFailed()
            .WithReadUserIdFromExpiredTokenSuccess(user.UserId)
            .WithFindByIdReturnsUser(user)
            .WithGenerateTokens(tokens);

        var handler = builder.Build();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.UserId.Should().Be(user.UserId);

        builder
            .VerifyGetRefreshTokenIsCalled()
            .VerifyGetAccessTokenIsCalled()
            .VerifyValidateAccessTokenIsCalled()
            .VerifyReadUserIdFromExpiredTokenIsCalled()
            .VerifyFindByIdIsCalled()
            .VerifyGenerateTokensIsCalled()
            .VerifySetTokenCookiesIsCalled()
            .VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_WhenAccessTokenExpiredAndUnreadable_ReturnsFailure()
    {
        // Arrange
        var command = new RefreshTokenCommand();

        var builder = new RefreshTokenCommandHandlerBuilder()
            .WithGetRefreshToken("valid-refresh-token")
            .WithGetAccessToken("corrupted-access-token")
            .WithValidateAccessTokenFailed()
            .WithReadUserIdFromExpiredTokenFailed(AuthenticationErrors.AccessTokenInvalidFormat);

        var handler = builder.Build();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();

        builder
            .VerifyGetRefreshTokenIsCalled()
            .VerifyGetAccessTokenIsCalled()
            .VerifyValidateAccessTokenIsCalled()
            .VerifyReadUserIdFromExpiredTokenIsCalled()
            .VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_WhenUserNotFound_ReturnsFailure()
    {
        // Arrange
        var userId = AuthenticationGenerator.UserId;
        var command = new RefreshTokenCommand();

        var builder = new RefreshTokenCommandHandlerBuilder()
            .WithGetRefreshToken("valid-refresh-token")
            .WithGetAccessToken("valid-access-token")
            .WithValidateAccessTokenSuccess(userId)
            .WithFindByIdReturnsNull();

        var handler = builder.Build();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        CustomFluentError.GetErrorCode(result.Errors.First()).Should().Be(AuthenticationErrors.UserNotFound.Code);

        builder
            .VerifyGetRefreshTokenIsCalled()
            .VerifyGetAccessTokenIsCalled()
            .VerifyValidateAccessTokenIsCalled()
            .VerifyFindByIdIsCalled()
            .VerifyNoOtherCalls();
    }
}
