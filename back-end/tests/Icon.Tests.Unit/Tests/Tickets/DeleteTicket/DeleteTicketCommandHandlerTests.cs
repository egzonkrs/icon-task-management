using FluentAssertions;
using Icon.Application.Features.Tickets.DeleteTicket;
using Icon.Domain.Common.Errors;
using Icon.SharedKernel.Common;
using Icon.Tests.Unit.Generators;

namespace Icon.Tests.Unit.Tests.Tickets.DeleteTicket;

public sealed class DeleteTicketCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithValidCommand_ReturnsSuccess()
    {
        // Arrange
        var userId = TicketGenerator.UserId;
        var ticket = TicketGenerator.CreateTicket(userId);

        var command = new DeleteTicketCommand { Id = ticket.Id };

        var builder = new DeleteTicketCommandHandlerBuilder()
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
            .VerifyRemoveIsCalled()
            .VerifySaveChangesIsCalled()
            .VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_WhenTicketNotFound_ReturnsFailure()
    {
        // Arrange
        var ticketId = Ulid.NewUlid();
        var command = new DeleteTicketCommand { Id = ticketId };

        var builder = new DeleteTicketCommandHandlerBuilder()
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

        var command = new DeleteTicketCommand { Id = ticket.Id };

        var builder = new DeleteTicketCommandHandlerBuilder()
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
}
