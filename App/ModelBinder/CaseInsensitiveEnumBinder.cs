using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace App.ModelBinder;

public class CaseInsensitiveEnumBinder<T> : IModelBinder where T : struct, Enum
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).FirstValue;

        if (string.IsNullOrWhiteSpace(value))
        {
            bindingContext.Result = ModelBindingResult.Failed();
            return Task.CompletedTask;
        }

        if (Enum.TryParse<T>(value, ignoreCase: true, out var parsed))
        {
            bindingContext.Result = ModelBindingResult.Success(parsed);
        }
        else
        {
            var valid = Enum.GetNames(typeof(T));
            var msg = $"Invalid value '{value}' for {typeof(T).Name}. Valid values are: {string.Join(", ", valid)}";
            bindingContext.ModelState.AddModelError(bindingContext.ModelName, msg);

            bindingContext.Result = ModelBindingResult.Failed();
        }

        return Task.CompletedTask;
    }
}