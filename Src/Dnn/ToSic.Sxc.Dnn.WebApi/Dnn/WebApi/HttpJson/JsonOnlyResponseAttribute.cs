using System;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http.Controllers;
using System.Web.Http.Dependencies;
using ToSic.Eav.Serialization;
using ToSic.Eav.WebApi.Serialization;

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
            {
                SetDefaultNewtonsoftJsonFormatter(controllerSettings);
                return;
            }

            // For newer apis we need to use System.Text.Json, but generated per request
            // because of DI dependencies for EavJsonConvertors in new generated JsonOptions
            SetSystemTextJsonFormatter(controllerSettings, controllerDescriptor.Configuration.DependencyResolver);
        }

        private void SetDefaultNewtonsoftJsonFormatter(HttpControllerSettings controllerSettings)
        {
            // Remove System.Text.Json JsonMediaTypeFormatter
            controllerSettings.Formatters.OfType<SystemTextJsonMediaTypeFormatter>().ToList()
                .ForEach(f => controllerSettings.Formatters.Remove(f));

            // Bring back original JsonFormatter
            if (!controllerSettings.Formatters.OfType<JsonMediaTypeFormatter>().Any())
                controllerSettings.Formatters.Insert(0, controllerSettings.Formatters.JsonFormatter);
        }

        private void SetSystemTextJsonFormatter(HttpControllerSettings controllerSettings, IDependencyResolver dependencyResolver)
        {
            // Remove default JsonMediaTypeFormatter (Newtonsoft)
            controllerSettings.Formatters.OfType<JsonMediaTypeFormatter>().ToList()
                .ForEach(f => controllerSettings.Formatters.Remove(f));

            // Set SystemTextJson JsonMediaTypeFormatter
            if (!controllerSettings.Formatters.OfType<SystemTextJsonMediaTypeFormatter>().Any())
                controllerSettings.Formatters.Insert(0, SystemTextJsonMediaTypeFormatterFactory(dependencyResolver));
        }

        private static SystemTextJsonMediaTypeFormatter SystemTextJsonMediaTypeFormatterFactory(IDependencyResolver dependencyResolver)
        {
            // Build Eav to Json converters for api v15
            var eavJsonConverterFactory = dependencyResolver.GetService(typeof(EavJsonConverterFactory)) as EavJsonConverterFactory;
            return new SystemTextJsonMediaTypeFormatter
            {
                JsonSerializerOptions = JsonOptions.UnsafeJsonWithoutEncodingHtmlOptionsFactory(eavJsonConverterFactory)
            };
        }
    }
}

