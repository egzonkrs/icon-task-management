using Icon.Application.Abstractions;
using Icon.Application.Features.Tickets.GetTickets;
using Icon.Domain.Entities;
using Icon.Domain.Repositories;
using Icon.SharedKernel.Specifications;
using Moq;

namespace Icon.Tests.Unit.Tests.Tickets.GetTickets;

internal sealed class GetTicketsQueryHandlerBuilder
{
    private readonly Mock<ITicketRepository> _ticketRepositoryMock = new();
    private readonly Mock<IUserContextAccessor> _userContextAccessorMock = new();

    public GetTicketsQueryHandlerBuilder WithAuthenticatedUser(string userId)
    {
        _userContextAccessorMock.Setup(x => x.UserId).Returns(userId);
        return this;
    }

    public GetTicketsQueryHandlerBuilder WithListReturnsTickets(IReadOnlyList<Ticket> tickets)
    {
        _ticketRepositoryMock
            .Setup(x => x.ListAsync(It.IsAny<ISpecification<Ticket>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tickets);
        _ticketRepositoryMock
            .Setup(x => x.ListAsync(It.IsAny<ISpecification<Ticket, Ticket>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tickets);
        return this;
    }

    public GetTicketsQueryHandlerBuilder WithListReturnsEmpty()
    {
        IReadOnlyList<Ticket> empty = Array.Empty<Ticket>();
        _ticketRepositoryMock
            .Setup(x => x.ListAsync(It.IsAny<ISpecification<Ticket>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(empty);
        _ticketRepositoryMock
            .Setup(x => x.ListAsync(It.IsAny<ISpecification<Ticket, Ticket>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(empty);
        return this;
    }

    public GetTicketsQueryHandlerBuilder VerifyListAsyncWithSpecificationIsCalled(int timesCalled = 1)
    {
        _ticketRepositoryMock.Verify(
            x => x.ListAsync(It.IsAny<ISpecification<Ticket, Ticket>>(), It.IsAny<CancellationToken>()),
            Times.Exactly(timesCalled));
        return this;
    }

    public GetTicketsQueryHandlerBuilder VerifyUserIdIsCalled(int timesCalled = 1)
    {
        _userContextAccessorMock.Verify(x => x.UserId, Times.Exactly(timesCalled));
        return this;
    }

    public void VerifyNoOtherCalls()
    {
        _ticketRepositoryMock.VerifyNoOtherCalls();
        _userContextAccessorMock.VerifyNoOtherCalls();
    }

    public GetTicketsQueryHandler Build()
    {
        return new(
            _ticketRepositoryMock.Object,
            _userContextAccessorMock.Object);
    }
}
