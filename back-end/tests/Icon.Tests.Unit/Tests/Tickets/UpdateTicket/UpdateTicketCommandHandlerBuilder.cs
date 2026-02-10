using Icon.Application.Abstractions;
using Icon.Application.Features.Tickets.UpdateTicket;
using Icon.Domain.Entities;
using Icon.Domain.Repositories;
using Icon.SharedKernel.Abstractions;
using Moq;

namespace Icon.Tests.Unit.Tests.Tickets.UpdateTicket;

internal sealed class UpdateTicketCommandHandlerBuilder
{
    private readonly Mock<ITicketRepository> _ticketRepositoryMock = new();
    private readonly Mock<IUserContextAccessor> _userContextAccessorMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    public UpdateTicketCommandHandlerBuilder WithAuthenticatedUser(string userId)
    {
        _userContextAccessorMock.Setup(x => x.UserId).Returns(userId);
        return this;
    }

    public UpdateTicketCommandHandlerBuilder WithGetByIdReturnsNull()
    {
        _ticketRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Ticket?)null);
        return this;
    }

    public UpdateTicketCommandHandlerBuilder WithGetByIdReturnsTicket(Ticket ticket)
    {
        _ticketRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);
        return this;
    }

    public UpdateTicketCommandHandlerBuilder WithSaveChangesReturns(int result)
    {
        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);
        return this;
    }

    public UpdateTicketCommandHandlerBuilder VerifyUpdateIsCalled(int timesCalled = 1)
    {
        _ticketRepositoryMock.Verify(
            x => x.Update(It.IsAny<Ticket>()),
            Times.Exactly(timesCalled));
        return this;
    }

    public UpdateTicketCommandHandlerBuilder VerifySaveChangesIsCalled(int timesCalled = 1)
    {
        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Exactly(timesCalled));
        return this;
    }

    public UpdateTicketCommandHandlerBuilder VerifyGetByIdIsCalled(int timesCalled = 1)
    {
        _ticketRepositoryMock.Verify(
            x => x.GetByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()),
            Times.Exactly(timesCalled));
        return this;
    }

    public UpdateTicketCommandHandlerBuilder VerifyUserIdIsCalled(int timesCalled = 1)
    {
        _userContextAccessorMock.Verify(x => x.UserId, Times.Exactly(timesCalled));
        return this;
    }

    public void VerifyNoOtherCalls()
    {
        _ticketRepositoryMock.VerifyNoOtherCalls();
        _unitOfWorkMock.VerifyNoOtherCalls();
        _userContextAccessorMock.VerifyNoOtherCalls();
    }

    public UpdateTicketCommandHandler Build()
    {
        return new(
            _ticketRepositoryMock.Object,
            _userContextAccessorMock.Object,
            _unitOfWorkMock.Object);
    }
}
