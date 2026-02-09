using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using Icon.SharedKernel.Common;
using MediatR;

namespace Icon.Application.Behaviors;

public sealed class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : ResultBase
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationPipelineBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var failures = await ValidateAsync(request, cancellationToken);

        if (failures.Length is 0)
        {
            return await next();
        }

        var errors = failures.Select(f => new CustomFluentError("VALIDATION_ERROR", f.ErrorMessage)).Cast<IError>();
        var failedResult = Result.Fail(errors);
        dynamic dynamicFailedResult = failedResult;
        TResponse typedResult = dynamicFailedResult;
        return typedResult;
    }

    private async Task<ValidationFailure[]> ValidateAsync(TRequest request, CancellationToken cancellationToken)
    {
        if (_validators.Any() is false) return [];

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        return validationResults.SelectMany(r => r.Errors).Where(f => f is not null).ToArray();
    }
}
