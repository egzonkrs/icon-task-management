using FluentAssertions;
using Icon.Application.Features.Tickets.GetTickets;
using Icon.Tests.Unit.Generators;

namespace Icon.Tests.Unit.Tests.Tickets.GetTickets;

public sealed class GetTicketsQueryHandlerTests
{
    [Fact]
    public async Task Handle_WithTickets_ReturnsEnvelopeWithItems()
    {
        // Arrange
        var userId = TicketGenerator.UserId;
        var tickets = TicketGenerator.CreateTickets(3, userId);

        var query = new GetTicketsQuery { IsCompleted = null, Search = null };

        var builder = new GetTicketsQueryHandlerBuilder()
            .WithAuthenticatedUser(userId)
            .WithListReturnsTickets(tickets);

        var handler = builder.Build();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().HaveCount(3);

        builder
            .VerifyUserIdIsCalled()
            .VerifyListAsyncWithSpecificationIsCalled()
            .VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_WithNoTickets_ReturnsEmptyEnvelope()
    {
        // Arrange
        var query = new GetTicketsQuery { IsCompleted = false, Search = "nonexistent" };

        var builder = new GetTicketsQueryHandlerBuilder()
            .WithAuthenticatedUser(TicketGenerator.UserId)
            .WithListReturnsEmpty();

        var handler = builder.Build();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().BeEmpty();

        builder
            .VerifyUserIdIsCalled()
            .VerifyListAsyncWithSpecificationIsCalled()
            .VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Handle_WithCompletedFilter_ReturnsSuccess()
    {
        // Arrange
        var userId = TicketGenerator.UserId;
        var completedTicket = TicketGenerator.CreateCompletedTicket(userId);

        var query = new GetTicketsQuery { IsCompleted = true };

        var builder = new GetTicketsQueryHandlerBuilder()
            .WithAuthenticatedUser(userId)
            .WithListReturnsTickets([completedTicket]);

        var handler = builder.Build();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Items.Should().HaveCount(1);

        builder
            .VerifyUserIdIsCalled()
            .VerifyListAsyncWithSpecificationIsCalled()
            .VerifyNoOtherCalls();
    }
}
