using FluentAssertions;
using Icon.Application.Features.Tickets.CompleteTicket;
using Icon.Domain.Common.Errors;
using Icon.SharedKernel.Common;
using Icon.Tests.Unit.Generators;

namespace Icon.Tests.Unit.Tests.Tickets.CompleteTicket;

public sealed class CompleteTicketCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidOpenTicket_ReturnsSuccess()
    {
        // Arrange
        var userId = TicketGenerator.UserId;
        var ticket = TicketGenerator.CreateTicket(userId);

        var command = new CompleteTicketCommand { Id = ticket.Id };

        var builder = new CompleteTicketCommandHandlerBuilder()
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
        var command = new CompleteTicketCommand { Id = ticketId };

        var builder = new CompleteTicketCommandHandlerBuilder()
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

        var command = new CompleteTicketCommand { Id = ticket.Id };

        var builder = new CompleteTicketCommandHandlerBuilder()
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
    public async Task Handle_WhenTicketAlreadyCompleted_ReturnsFailure()
    {
        // Arrange
        var userId = TicketGenerator.UserId;
        var ticket = TicketGenerator.CreateCompletedTicket(userId);

        var command = new CompleteTicketCommand { Id = ticket.Id };

        var builder = new CompleteTicketCommandHandlerBuilder()
            .WithAuthenticatedUser(userId)
            .WithGetByIdReturnsTicket(ticket);

        var handler = builder.Build();

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        CustomFluentError.GetErrorCode(result.Errors.First()).Should().Be(TicketErrors.AlreadyCompleted().Code);

        builder
            .VerifyGetByIdIsCalled()
            .VerifyUserIdIsCalled()
            .VerifyNoOtherCalls();
    }
}
