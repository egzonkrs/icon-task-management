using FluentResults;
using MediatR;
using Icon.Application.Abstractions;
using Icon.Application.Features.Authentication.Common;
using Icon.Domain.Common.Errors;

namespace Icon.Application.Features.Authentication.GetCurrentUser;

public sealed class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, Result<AuthenticationResponse>>
{
    private readonly IUserContextAccessor _userContextAccessor;
    public GetCurrentUserQueryHandler(IUserContextAccessor userContextAccessor) { _userContextAccessor = userContextAccessor; }

    public Task<Result<AuthenticationResponse>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var userId = _userContextAccessor.UserId;
        if (string.IsNullOrEmpty(userId))
        {
            return Task.FromResult(Result.Fail<AuthenticationResponse>(AuthenticationErrors.Unauthenticated));
        }

        var response = new AuthenticationResponse
        {
            UserId = userId,
            Email = _userContextAccessor.Email,
            FullName = _userContextAccessor.Name
        };
        return Task.FromResult(Result.Ok(response));
    }
}
