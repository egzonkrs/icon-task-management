using FluentAssertions;
using Icon.Application.Features.Tickets.UpdateTicket;
using Icon.Domain.Common.Errors;
using Icon.SharedKernel.Common;
using Icon.Tests.Unit.Generators;

namespace Icon.Tests.Unit.Tests.Tickets.UpdateTicket;

public sealed class UpdateTicketCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_ReturnsSuccess()
    {
        // Arrange
        var userId = TicketGenerator.UserId;
        var ticket = TicketGenerator.CreateTicket(userId);

        var command = new UpdateTicketCommand
        {
            Id = ticket.Id,
            Title = TicketGenerator.Title,
            Description = TicketGenerator.Description,
            DueDate = DateTime.UtcNow.AddDays(14)
        };

        var builder = new UpdateTicketCommandHandlerBuilder()
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
    public async Task Handle_WhenTicketNotFound_ReturnsFailure()
    {
        // Arrange
        var ticketId = Ulid.NewUlid();

        var command = new UpdateTicketCommand
        {
            Id = ticketId,
            Title = TicketGenerator.Title
        };

        var builder = new UpdateTicketCommandHandlerBuilder()
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

        var command = new UpdateTicketCommand
        {
            Id = ticket.Id,
            Title = TicketGenerator.Title
        };

        var builder = new UpdateTicketCommandHandlerBuilder()
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
    public async Task Handle_WhenSaveChangesReturnsZero_ReturnsFailure()
    {
        // Arrange
        var userId = TicketGenerator.UserId;
        var ticket = TicketGenerator.CreateTicket(userId);

        var command = new UpdateTicketCommand
        {
            Id = ticket.Id,
            Title = TicketGenerator.Title,
            Description = TicketGenerator.Description
        };

        var builder = new UpdateTicketCommandHandlerBuilder()
            .WithAuthenticatedUser(userId)
            .WithGetByIdReturnsTicket(ticket)
            .WithSaveChangesReturns(0);

        var handler = builder.Build();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        CustomFluentError.GetErrorCode(result.Errors.First()).Should().Be(TicketErrors.NoChangesDetected().Code);

        builder
            .VerifyGetByIdIsCalled()
            .VerifyUserIdIsCalled()
            .VerifyUpdateIsCalled()
            .VerifySaveChangesIsCalled()
            .VerifyNoOtherCalls();
    }
}
