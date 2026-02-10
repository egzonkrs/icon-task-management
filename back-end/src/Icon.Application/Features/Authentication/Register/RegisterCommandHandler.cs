using FluentResults;
using MediatR;
using Icon.Application.Abstractions.Authentication;
using Icon.Application.Features.Authentication.Common;
using Icon.Domain.Common.Errors;

namespace Icon.Application.Features.Authentication.Register;

public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthenticationResponse>>
{
    private readonly IIdentityService _identityService;

    public RegisterCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<Result<AuthenticationResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _identityService.FindByEmailAsync(request.Email, cancellationToken);
        if (existingUser is not null)
        {
            return Result.Fail(AuthenticationErrors.EmailAlreadyExists);
        }

        var createResult = await _identityService.CreateUserAsync(request.Email, request.Password, request.FirstName, request.LastName, cancellationToken);
        if (createResult.IsFailed)
        {
            return createResult.ToResult<AuthenticationResponse>();
        }

        var user = createResult.Value;

        return Result.Ok(new AuthenticationResponse
        {
            UserId = user.UserId,
            Email = user.Email,
            FullName = user.FullName
        });
    }
}
