using Icon.Application.Abstractions;
using Icon.Application.Features.Tickets.GetTicketById;
using Icon.Domain.Entities;
using Icon.Domain.Repositories;
using Moq;

namespace Icon.Tests.Unit.Tests.Tickets.GetTicketById;

internal sealed class GetTicketByIdQueryHandlerBuilder
{
    private readonly Mock<ITicketRepository> _ticketRepositoryMock = new();
    private readonly Mock<IUserContextAccessor> _userContextAccessorMock = new();

    public GetTicketByIdQueryHandlerBuilder WithAuthenticatedUser(string userId)
    {
        _userContextAccessorMock.Setup(x => x.UserId).Returns(userId);
        return this;
    }

    public GetTicketByIdQueryHandlerBuilder WithGetByIdReturnsNull()
    {
        _ticketRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Ticket?)null);
        return this;
    }

    public GetTicketByIdQueryHandlerBuilder WithGetByIdReturnsTicket(Ticket ticket)
    {
        _ticketRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);
        return this;
    }

    public GetTicketByIdQueryHandlerBuilder VerifyGetByIdIsCalled(int timesCalled = 1)
    {
        _ticketRepositoryMock.Verify(
            x => x.GetByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()),
            Times.Exactly(timesCalled));
        return this;
    }

    public GetTicketByIdQueryHandlerBuilder VerifyUserIdIsCalled(int timesCalled = 1)
    {
        _userContextAccessorMock.Verify(x => x.UserId, Times.Exactly(timesCalled));
        return this;
    }

    public void VerifyNoOtherCalls()
    {
        _ticketRepositoryMock.VerifyNoOtherCalls();
        _userContextAccessorMock.VerifyNoOtherCalls();
    }

    public GetTicketByIdQueryHandler Build()
    {
        return new(
            _ticketRepositoryMock.Object,
            _userContextAccessorMock.Object);
    }
}
