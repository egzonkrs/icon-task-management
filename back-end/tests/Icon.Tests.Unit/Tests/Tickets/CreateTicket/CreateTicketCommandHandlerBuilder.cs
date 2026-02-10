using Icon.Application.Abstractions;
using Icon.Application.Features.Tickets.CreateTicket;
using Icon.Domain.Entities;
using Icon.Domain.Repositories;
using Icon.SharedKernel.Abstractions;
using Moq;

namespace Icon.Tests.Unit.Tests.Tickets.CreateTicket;

internal sealed class CreateTicketCommandHandlerBuilder
{
    private readonly Mock<ITicketRepository> _ticketRepositoryMock = new();
    private readonly Mock<IUserContextAccessor> _userContextAccessorMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    public CreateTicketCommandHandlerBuilder WithAuthenticatedUser(string userId)
    {
        _userContextAccessorMock.Setup(x => x.UserId).Returns(userId);
        return this;
    }

    public CreateTicketCommandHandlerBuilder WithGetByTitleReturnsNull()
    {
        _ticketRepositoryMock
            .Setup(x => x.GetByTitleAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Ticket?)null);
        return this;
    }

    public CreateTicketCommandHandlerBuilder WithGetByTitleReturnsTicket(Ticket ticket)
    {
        _ticketRepositoryMock
            .Setup(x => x.GetByTitleAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);
        return this;
    }

    public CreateTicketCommandHandlerBuilder WithSaveChangesReturns(int result)
    {
        _unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);
        return this;
    }

    public CreateTicketCommandHandlerBuilder VerifyAddAsyncIsCalled(int timesCalled = 1)
    {
        _ticketRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Ticket>(), It.IsAny<CancellationToken>()),
            Times.Exactly(timesCalled));
        return this;
    }

    public CreateTicketCommandHandlerBuilder VerifySaveChangesIsCalled(int timesCalled = 1)
    {
        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Exactly(timesCalled));
        return this;
    }

    public CreateTicketCommandHandlerBuilder VerifyGetByTitleIsCalled(int timesCalled = 1)
    {
        _ticketRepositoryMock.Verify(
            x => x.GetByTitleAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Exactly(timesCalled));
        return this;
    }

    public CreateTicketCommandHandlerBuilder VerifyUserIdIsCalled(int timesCalled = 1)
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

    public CreateTicketCommandHandler Build()
    {
        return new(
            _ticketRepositoryMock.Object,
            _userContextAccessorMock.Object,
            _unitOfWorkMock.Object);
    }
}
