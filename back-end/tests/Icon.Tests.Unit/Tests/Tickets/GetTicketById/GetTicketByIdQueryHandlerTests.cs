using FluentAssertions;
using Icon.Application.Features.Tickets.GetTicketById;
using Icon.Domain.Common.Errors;
using Icon.SharedKernel.Common;
using Icon.Tests.Unit.Generators;

namespace Icon.Tests.Unit.Tests.Tickets.GetTicketById;

public sealed class GetTicketByIdQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithValidId_ReturnsTicketDetail()
    {
        // Arrange
        var userId = TicketGenerator.UserId;
        var ticket = TicketGenerator.CreateTicket(userId);

        var query = new GetTicketByIdQuery { Id = ticket.Id };

        var builder = new GetTicketByIdQueryHandlerBuilder()
            .WithAuthenticatedUser(userId)
            .WithGetByIdReturnsTicket(ticket);

        var handler = builder.Build();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(ticket.Id.ToString());
        result.Value.Title.Should().Be(ticket.Title.Value);

        builder
            .VerifyGetByIdIsCalled()
            .VerifyUserIdIsCalled()
            .VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_WhenTicketNotFound_ReturnsFailure()
    {
        // Arrange
        var ticketId = Ulid.NewUlid();
        var query = new GetTicketByIdQuery { Id = ticketId };

        var builder = new GetTicketByIdQueryHandlerBuilder()
            .WithAuthenticatedUser(TicketGenerator.UserId)
            .WithGetByIdReturnsNull();

        var handler = builder.Build();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

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

        var query = new GetTicketByIdQuery { Id = ticket.Id };

        var builder = new GetTicketByIdQueryHandlerBuilder()
            .WithAuthenticatedUser(differentUserId)
            .WithGetByIdReturnsTicket(ticket);

        var handler = builder.Build();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        CustomFluentError.GetErrorCode(result.Errors.First()).Should().Be(TicketErrors.Unauthorized().Code);

        builder
            .VerifyGetByIdIsCalled()
            .VerifyUserIdIsCalled()
            .VerifyNoOtherCalls();
    }
}
