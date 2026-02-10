using FluentResults;
using MediatR;
using Icon.Application.Abstractions.Authentication;

namespace Icon.Application.Features.Authentication.Logout;

public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result<bool>>
{
    private readonly IJwtCookieService _jwtCookieService;
    public LogoutCommandHandler(IJwtCookieService jwtCookieService) { _jwtCookieService = jwtCookieService; }

    public Task<Result<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        _jwtCookieService.ClearTokenCookies();
        return Task.FromResult(Result.Ok(true));
    }
}
