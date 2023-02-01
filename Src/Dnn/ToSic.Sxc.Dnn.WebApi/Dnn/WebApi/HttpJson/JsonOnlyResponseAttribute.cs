using System;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http.Controllers;
using ToSic.Eav.Serialization;
using ToSic.Eav.WebApi.Serialization;
using ToSic.Sxc.WebApi;

// Special case: this should enforce json formatting
// It's only needed in .net4x where the default is xml
namespace ToSic.Sxc.Dnn.WebApi.HttpJson
{
    public class JsonOnlyResponseAttribute : Attribute, IControllerConfiguration
    {
        public void Initialize(HttpControllerSettings controllerSettings, HttpControllerDescriptor controllerDescriptor)
        {
            // Remove the XML formatter
            controllerSettings.Formatters.Remove(controllerSettings.Formatters.XmlFormatter);
            
            // For older apis we need to leave NewtonsoftJson
            if (GetCustomAttributes(controllerDescriptor.ControllerType).OfType<UseOldNewtonsoftForHttpJsonAttribute>().Any())
                return;

            // For newer apis we need to use System.Text.Json, but generated per request
            // because of DI dependencies for EavJsonConvertors in new generated JsonOptions
            SetSystemTextJsonFormatter(controllerSettings, controllerDescriptor);
        }

        private void SetSystemTextJsonFormatter(HttpControllerSettings controllerSettings, HttpControllerDescriptor controllerDescriptor)
        {
            // Remove default JsonMediaTypeFormatter (Newtonsoft)
            controllerSettings.Formatters.OfType<JsonMediaTypeFormatter>().ToList()
                .ForEach(f => controllerSettings.Formatters.Remove(f));

            // Set SystemTextJson JsonMediaTypeFormatter
            if (!controllerSettings.Formatters.OfType<SystemTextJsonMediaTypeFormatter>().Any())
                controllerSettings.Formatters.Insert(0, SystemTextJsonMediaTypeFormatterFactory(controllerDescriptor));
        }

        private static SystemTextJsonMediaTypeFormatter SystemTextJsonMediaTypeFormatterFactory(HttpControllerDescriptor controllerDescriptor)
        {
            var jsonFormatterAttribute = GetCustomAttributes(controllerDescriptor.ControllerType).OfType<JsonFormatterAttribute>().FirstOrDefault();

            // Build Eav to Json converters for api v15
            var eavJsonConverterFactory = (jsonFormatterAttribute?.AutoConvertEntity == false) ? null : 
                controllerDescriptor.Configuration.DependencyResolver.GetService(typeof(EavJsonConverterFactory)) as EavJsonConverterFactory;

            var jsonSerializerOptions = JsonOptions.UnsafeJsonWithoutEncodingHtmlOptionsFactory(eavJsonConverterFactory);

            if (jsonFormatterAttribute?.Casing == Casing.CamelCase) 
                jsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;

            return new SystemTextJsonMediaTypeFormatter { JsonSerializerOptions = jsonSerializerOptions };
        }
    }
}

