using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

//https://thomaslevesque.com/2018/09/04/handling-multipart-requests-with-json-and-file-uploads-in-asp-net-core/
// Slightly modified to also support regular POSTs with a typical body
namespace ReuseventoryApi.Helpers
{
    public class JsonWithFilesFormDataModelBinder : IModelBinder
    {
        private readonly FormFileModelBinder _formFileModelBinder;

        public JsonWithFilesFormDataModelBinder(ILoggerFactory loggerFactory)
        {
            _formFileModelBinder = new FormFileModelBinder(loggerFactory);
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            // Retrieve the form part containing the JSON
            var valueResult = bindingContext.ValueProvider.GetValue(bindingContext.FieldName);

            string rawValue;
            if (valueResult == ValueProviderResult.None)
            {
                //if we didn't find the content in the multi-part form, try looking in the body
                using (var reader = new StreamReader(bindingContext.HttpContext.Request.Body))
                {
                    rawValue = await reader.ReadToEndAsync();
                }

                if (String.IsNullOrEmpty(rawValue))
                {
                    // The JSON was not found in the form or the body
                    var message = bindingContext.ModelMetadata.ModelBindingMessageProvider.MissingBindRequiredValueAccessor(bindingContext.FieldName);
                    bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, message);
                    return;
                }
            }
            else
            {
                rawValue = valueResult.FirstValue;
            }


            // Deserialize the JSON
            var model = JsonConvert.DeserializeObject(rawValue, bindingContext.ModelType);

            // Now, bind each of the IFormFile properties from the other form parts
            foreach (var property in bindingContext.ModelMetadata.Properties)
            {
                if (property.ModelType != typeof(IFormFile))
                    continue;

                var fieldName = property.BinderModelName ?? property.PropertyName;
                var modelName = fieldName;
                var propertyModel = property.PropertyGetter(bindingContext.Model);
                ModelBindingResult propertyResult;
                using (bindingContext.EnterNestedScope(property, fieldName, modelName, propertyModel))
                {
                    await _formFileModelBinder.BindModelAsync(bindingContext);
                    propertyResult = bindingContext.Result;
                }

                if (propertyResult.IsModelSet)
                {
                    // The IFormFile was sucessfully bound, assign it to the corresponding property of the model
                    property.PropertySetter(model, propertyResult.Model);
                }
                else if (property.IsBindingRequired)
                {
                    var message = property.ModelBindingMessageProvider.MissingBindRequiredValueAccessor(fieldName);
                    bindingContext.ModelState.TryAddModelError(modelName, message);
                }
            }

            // Set the successfully constructed model as the result of the model binding
            bindingContext.Result = ModelBindingResult.Success(model);
        }
    }
}