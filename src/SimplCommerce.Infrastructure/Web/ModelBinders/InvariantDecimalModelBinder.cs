﻿using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Logging;

namespace SimplCommerce.Infrastructure.Web.ModelBinders
{
    public class InvariantDecimalModelBinder : IModelBinder
    {
        private readonly ILoggerFactory _loggerFactory;

        public InvariantDecimalModelBinder(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueProviderResult != ValueProviderResult.None)
            {
                bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

                var valueAsString = valueProviderResult.FirstValue;
                decimal result;

                if (decimal.TryParse(valueAsString, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out result))
                {
                    bindingContext.Result = ModelBindingResult.Success(result);
                    return Task.CompletedTask;
                }
            }

            // If we haven't handled it, then we'll let the base SimpleTypeModelBinder handle it
            var baseBinder = new SimpleTypeModelBinder(bindingContext.ModelType, _loggerFactory);
            return baseBinder.BindModelAsync(bindingContext);
        }
    }
}
