using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Icon.Api.Common.ModelBinding;

/// <summary>
/// Model binder for Ulid type that handles string to Ulid conversion.
/// </summary>
public sealed class UlidModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        var modelName = bindingContext.ModelName;
        var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

        if (valueProviderResult == ValueProviderResult.None)
        {
            return Task.CompletedTask;
        }

        bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);

        var value = valueProviderResult.FirstValue;

        if (string.IsNullOrEmpty(value))
        {
            return Task.CompletedTask;
        }

        if (Ulid.TryParse(value, out var ulid))
        {
            bindingContext.Result = ModelBindingResult.Success(ulid);
        }
        else
        {
            bindingContext.ModelState.TryAddModelError(modelName, "Invalid ULID format");
        }

        return Task.CompletedTask;
    }
}
