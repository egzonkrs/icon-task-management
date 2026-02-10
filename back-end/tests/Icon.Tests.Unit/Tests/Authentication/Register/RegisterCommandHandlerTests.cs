using FluentAssertions;
using FluentResults;
using Icon.Application.Features.Authentication.Register;
using Icon.Domain.Common.Errors;
using Icon.SharedKernel.Common;
using Icon.Tests.Unit.Generators;

namespace Icon.Tests.Unit.Tests.Authentication.Register;

public sealed class RegisterCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_ReturnsAuthenticationResponse()
    {
        // Arrange
        var user = AuthenticationGenerator.CreateIdentityUserModel();

        var command = new RegisterCommand
        {
            Email = user.Email,
            Password = AuthenticationGenerator.Password,
            FirstName = AuthenticationGenerator.FirstName,
            LastName = AuthenticationGenerator.LastName
        };

        var builder = new RegisterCommandHandlerBuilder()
            .WithFindByEmailReturnsNull()
            .WithCreateUserSuccess(user);

        var handler = builder.Build();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.UserId.Should().Be(user.UserId);
        result.Value.Email.Should().Be(user.Email);
        result.Value.FullName.Should().Be(user.FullName);

        builder
            .VerifyFindByEmailIsCalled()
            .VerifyCreateUserIsCalled()
            .VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_WhenEmailAlreadyExists_ReturnsFailure()
    {
        // Arrange
        var existingUser = AuthenticationGenerator.CreateIdentityUserModel();

        var command = new RegisterCommand
        {
            Email = existingUser.Email,
            Password = AuthenticationGenerator.Password,
            FirstName = AuthenticationGenerator.FirstName,
            LastName = AuthenticationGenerator.LastName
        };

        var builder = new RegisterCommandHandlerBuilder()
            .WithFindByEmailReturnsUser(existingUser);

        var handler = builder.Build();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        CustomFluentError.GetErrorCode(result.Errors.First()).Should().Be(AuthenticationErrors.EmailAlreadyExists.Code);

        builder
            .VerifyFindByEmailIsCalled()
            .VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_WhenCreateUserFails_ReturnsFailure()
    {
        // Arrange
        var command = new RegisterCommand
        {
            Email = AuthenticationGenerator.Email,
            Password = AuthenticationGenerator.Password,
            FirstName = AuthenticationGenerator.FirstName,
            LastName = AuthenticationGenerator.LastName
        };

        var error = new Error("CREATE_USER_FAILED");

        var builder = new RegisterCommandHandlerBuilder()
            .WithFindByEmailReturnsNull()
            .WithCreateUserFailed(error);

        var handler = builder.Build();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();

        builder
            .VerifyFindByEmailIsCalled()
            .VerifyCreateUserIsCalled()
            .VerifyNoOtherCalls();
    }
}
