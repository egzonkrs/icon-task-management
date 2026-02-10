using FluentAssertions;
using Icon.Application.Features.Tickets.CreateTicket;
using Icon.Domain.Common.Errors;
using Icon.SharedKernel.Common;
using Icon.Tests.Unit.Generators;

namespace Icon.Tests.Unit.Tests.Tickets.CreateTicket;

public sealed class CreateTicketCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_ReturnsSuccessWithTicketId()
    {
        // Arrange
        var userId = TicketGenerator.UserId;

        var command = new CreateTicketCommand
        {
            Title = TicketGenerator.Title,
            Description = TicketGenerator.Description,
            Priority = "High",
            DueDate = DateTime.UtcNow.AddDays(7)
        };

        var builder = new CreateTicketCommandHandlerBuilder()
            .WithAuthenticatedUser(userId)
            .WithGetByTitleReturnsNull()
            .WithSaveChangesReturns(1);

        var handler = builder.Build();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Ulid.Empty);

        builder
            .VerifyUserIdIsCalled()
            .VerifyGetByTitleIsCalled()
            .VerifyAddAsyncIsCalled()
            .VerifySaveChangesIsCalled()
            .VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_WithNullDescription_ReturnsSuccess()
    {
        // Arrange
        var userId = TicketGenerator.UserId;

        var command = new CreateTicketCommand
        {
            Title = TicketGenerator.Title,
            Description = null,
            Priority = null,
            DueDate = null
        };

        var builder = new CreateTicketCommandHandlerBuilder()
            .WithAuthenticatedUser(userId)
            .WithGetByTitleReturnsNull()
            .WithSaveChangesReturns(1);

        var handler = builder.Build();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Ulid.Empty);

        builder
            .VerifyUserIdIsCalled()
            .VerifyGetByTitleIsCalled()
            .VerifyAddAsyncIsCalled()
            .VerifySaveChangesIsCalled()
            .VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_WithDuplicateTitle_ReturnsFailure()
    {
        // Arrange
        var userId = TicketGenerator.UserId;
        var existingTicket = TicketGenerator.CreateTicket(userId);

        var command = new CreateTicketCommand
        {
            Title = TicketGenerator.Title,
            Description = TicketGenerator.Description
        };

        var builder = new CreateTicketCommandHandlerBuilder()
            .WithAuthenticatedUser(userId)
            .WithGetByTitleReturnsTicket(existingTicket)
            .WithSaveChangesReturns(1);

        var handler = builder.Build();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        CustomFluentError.GetErrorCode(result.Errors.First()).Should().Be(TicketErrors.AlreadyExists(command.Title).Code);

        builder
            .VerifyUserIdIsCalled()
            .VerifyGetByTitleIsCalled()
            .VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_WhenSaveChangesReturnsZero_ReturnsFailure()
    {
        // Arrange
        var userId = TicketGenerator.UserId;

        var command = new CreateTicketCommand
        {
            Title = TicketGenerator.Title,
            Description = TicketGenerator.Description
        };

        var builder = new CreateTicketCommandHandlerBuilder()
            .WithAuthenticatedUser(userId)
            .WithGetByTitleReturnsNull()
            .WithSaveChangesReturns(0);

        var handler = builder.Build();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        CustomFluentError.GetErrorCode(result.Errors.First()).Should().Be(TicketErrors.NoChangesDetected().Code);

        builder
            .VerifyUserIdIsCalled()
            .VerifyGetByTitleIsCalled()
            .VerifyAddAsyncIsCalled()
            .VerifySaveChangesIsCalled()
            .VerifyNoOtherCalls();
    }
}
