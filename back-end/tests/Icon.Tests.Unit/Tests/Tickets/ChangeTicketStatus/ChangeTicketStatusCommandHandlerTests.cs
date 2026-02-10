using FluentAssertions;
using Icon.Application.Features.Tickets.ChangeTicketStatus;
using Icon.Domain.Common.Errors;
using Icon.SharedKernel.Common;
using Icon.Tests.Unit.Generators;

namespace Icon.Tests.Unit.Tests.Tickets.ChangeTicketStatus;

public sealed class ChangeTicketStatusCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidStatusTransition_ReturnsSuccess()
    {
        // Arrange
        var userId = TicketGenerator.UserId;
        var ticket = TicketGenerator.CreateTicket(userId);

        var command = new ChangeTicketStatusCommand
        {
            Id = ticket.Id,
            Status = "InProgress"
        };

        var builder = new ChangeTicketStatusCommandHandlerBuilder()
            .WithAuthenticatedUser(userId)
            .WithGetByIdReturnsTicket(ticket)
            .WithSaveChangesReturns(1);

        var handler = builder.Build();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();

        builder
            .VerifyGetByIdIsCalled()
            .VerifyUserIdIsCalled()
            .VerifyUpdateIsCalled()
            .VerifySaveChangesIsCalled()
            .VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_WithCaseInsensitiveStatus_ReturnsSuccess()
    {
        // Arrange
        var userId = TicketGenerator.UserId;
        var ticket = TicketGenerator.CreateTicket(userId);

        var command = new ChangeTicketStatusCommand
        {
            Id = ticket.Id,
            Status = "done"
        };

        var builder = new ChangeTicketStatusCommandHandlerBuilder()
            .WithAuthenticatedUser(userId)
            .WithGetByIdReturnsTicket(ticket)
            .WithSaveChangesReturns(1);

        var handler = builder.Build();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        builder
            .VerifyGetByIdIsCalled()
            .VerifyUserIdIsCalled()
            .VerifyUpdateIsCalled()
            .VerifySaveChangesIsCalled()
            .VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_WhenTicketNotFound_ReturnsFailure()
    {
        // Arrange
        var ticketId = Ulid.NewUlid();

        var command = new ChangeTicketStatusCommand
        {
            Id = ticketId,
            Status = "InProgress"
        };

        var builder = new ChangeTicketStatusCommandHandlerBuilder()
            .WithAuthenticatedUser(TicketGenerator.UserId)
            .WithGetByIdReturnsNull();

        var handler = builder.Build();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        CustomFluentError.GetErrorCode(result.Errors.First()).Should().Be(TicketErrors.NotFound(ticketId).Code);

        builder
            .VerifyGetByIdIsCalled()
            .VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_WhenUserIsNotOwner_ReturnsUnauthorized()
    {
        // Arrange
        var ticket = TicketGenerator.CreateTicket();
        var differentUserId = TicketGenerator.UserId;

        var command = new ChangeTicketStatusCommand
        {
            Id = ticket.Id,
            Status = "InProgress"
        };

        var builder = new ChangeTicketStatusCommandHandlerBuilder()
            .WithAuthenticatedUser(differentUserId)
            .WithGetByIdReturnsTicket(ticket);

        var handler = builder.Build();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        CustomFluentError.GetErrorCode(result.Errors.First()).Should().Be(TicketErrors.Unauthorized().Code);

        builder
            .VerifyGetByIdIsCalled()
            .VerifyUserIdIsCalled()
            .VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_WithInvalidStatus_ReturnsFailure()
    {
        // Arrange
        var userId = TicketGenerator.UserId;
        var ticket = TicketGenerator.CreateTicket(userId);

        var command = new ChangeTicketStatusCommand
        {
            Id = ticket.Id,
            Status = "InvalidStatus"
        };

        var builder = new ChangeTicketStatusCommandHandlerBuilder()
            .WithAuthenticatedUser(userId)
            .WithGetByIdReturnsTicket(ticket);

        var handler = builder.Build();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        CustomFluentError.GetErrorCode(result.Errors.First()).Should().Be(TicketErrors.InvalidStatus(command.Status).Code);

        builder
            .VerifyGetByIdIsCalled()
            .VerifyUserIdIsCalled()
            .VerifyNoOtherCalls();
    }
}
