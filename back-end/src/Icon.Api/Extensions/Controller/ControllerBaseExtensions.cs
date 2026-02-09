using System.Net;
using FluentResults;
using Icon.Api.Contracts.Common;
using Icon.SharedKernel.Common;
using Microsoft.AspNetCore.Mvc;

namespace Icon.Api.Extensions.Controller;

/// <summary>
/// Extensions for converting FluentResults into ActionResult responses.
/// </summary>
public static class ControllerBaseExtensions
{
    /// <summary>
    /// Creates an ActionResult from a Result{T} by mapping it to an ApiResponse{T}.
    /// </summary>
    public static IActionResult ToActionResult<TData>(this ControllerBase controller, Result<TData> result, HttpStatusCode successCode = HttpStatusCode.OK)
    {
        if (result.IsSuccess)
        {
            return controller.StatusCode((int)successCode, new ApiResponse<TData>
            {
                Data = result.ValueOrDefault,
                IsFailed = result.IsFailed,
                IsSuccess = result.IsSuccess,
                Reasons = result.Reasons.ToCodeMessageDictionary(),
                Errors = result.Errors.ToCodeMessageDictionary()
            });
        }

        if (result.IsFailed && result.HasException<Exception>())
        {
            return controller.StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse<TData>
            {
                IsFailed = result.IsFailed,
                IsSuccess = result.IsSuccess,
                Errors = result.Errors.ToCodeMessageDictionary(),
            });
        }

        var isNotFound = result.Errors.Any(err => err is CustomFluentError cfe &&
            cfe.Code.EndsWith("_NOT_FOUND", StringComparison.OrdinalIgnoreCase));

        if (result.IsFailed && isNotFound)
        {
            return controller.StatusCode((int)HttpStatusCode.NotFound, new ApiResponse<TData>
            {
                IsFailed = result.IsFailed,
                IsSuccess = result.IsSuccess,
                Errors = result.Errors.ToCodeMessageDictionary(),
            });
        }

        return controller.BadRequest(new ApiResponse<TData>
        {
            IsFailed = result.IsFailed,
            IsSuccess = result.IsSuccess,
            Errors = result.Errors.ToCodeMessageDictionary(),
        });
    }

    private static Dictionary<string, string> ToCodeMessageDictionary(this IEnumerable<IError> errors)
    {
        return errors
            .Select(error => error is CustomFluentError cfe
                ? new { Code = cfe.Code, cfe.Message }
                : new { Code = error.Message, error.Message })
            .GroupBy(item => item.Code)
            .ToDictionary(group => group.Key, group => group.First().Message);
    }

    private static Dictionary<string, string> ToCodeMessageDictionary(this IEnumerable<IReason> reasons)
    {
        return reasons
            .Select(reason => reason is CustomFluentError cfe
                ? new { Code = cfe.Code, cfe.Message }
                : new { Code = reason.Message, reason.Message })
            .GroupBy(item => item.Code)
            .ToDictionary(group => group.Key, group => group.First().Message);
    }
}
