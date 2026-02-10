using Icon.Application.Abstractions;
using Icon.Application.Features.Tickets.CompleteTicket;
using Icon.Domain.Entities;
using Icon.Domain.Repositories;
using Icon.SharedKernel.Abstractions;
using Moq;

namespace Icon.Tests.Unit.Tests.Tickets.CompleteTicket;

internal sealed class CompleteTicketCommandHandlerBuilder
{
    private readonly Mock<ITicketRepository> _ticketRepositoryMock = new();
    private readonly Mock<IUserContextAccessor> _userContextAccessorMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    public CompleteTicketCommandHandlerBuilder WithAuthenticatedUser(string userId)
    {
        _userContextAccessorMock.Setup(x => x.UserId).Returns(userId);
        return this;
    }

    public CompleteTicketCommandHandlerBuilder WithGetByIdReturnsNull()
    {
        _ticketRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Ticket?)null);
        return this;
    }

    public CompleteTicketCommandHandlerBuilder WithGetByIdReturnsTicket(Ticket ticket)
    {
        _ticketRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);
        return this;
    }

    public CompleteTicketCommandHandlerBuilder WithSaveChangesReturns(int result)
    {
        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);
        return this;
    }

    public CompleteTicketCommandHandlerBuilder VerifyUpdateIsCalled(int timesCalled = 1)
    {
        _ticketRepositoryMock.Verify(
            x => x.Update(It.IsAny<Ticket>()),
            Times.Exactly(timesCalled));
        return this;
    }

    public CompleteTicketCommandHandlerBuilder VerifySaveChangesIsCalled(int timesCalled = 1)
    {
        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Exactly(timesCalled));
        return this;
    }

    public CompleteTicketCommandHandlerBuilder VerifyGetByIdIsCalled(int timesCalled = 1)
    {
        _ticketRepositoryMock.Verify(
            x => x.GetByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()),
            Times.Exactly(timesCalled));
        return this;
    }

    public CompleteTicketCommandHandlerBuilder VerifyUserIdIsCalled(int timesCalled = 1)
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

    public CompleteTicketCommandHandler Build()
    {
        return new(_ticketRepositoryMock.Object, _userContextAccessorMock.Object, _unitOfWorkMock.Object);
    }
}
