using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Icon.Api.Common.ModelBinding;

/// <summary>
/// Model binder provider for Ulid type.
/// </summary>
public sealed class UlidModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Metadata.ModelType == typeof(Ulid))
        {
            return new BinderTypeModelBinder(typeof(UlidModelBinder));
        }

        return null;
    }
}
