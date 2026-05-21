using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FormatChanger.Infrastructure.ModelBinders
{
    public class FloatModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var vpr = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            if (vpr == ValueProviderResult.None)
                return Task.CompletedTask;

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, vpr);

            var raw = vpr.FirstValue;
            if (string.IsNullOrWhiteSpace(raw))
            {
                bindingContext.Result = ModelBindingResult.Success(0f);
                return Task.CompletedTask;
            }

            var normalized = raw.Replace(',', '.');
            if (float.TryParse(normalized, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
                bindingContext.Result = ModelBindingResult.Success(value);
            else
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, $"«{raw}» — не является числом.");

            return Task.CompletedTask;
        }
    }

    public class FloatModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            ArgumentNullException.ThrowIfNull(context);
            return context.Metadata.ModelType == typeof(float) || context.Metadata.ModelType == typeof(float?)
                ? new FloatModelBinder()
                : null;
        }
    }
}
