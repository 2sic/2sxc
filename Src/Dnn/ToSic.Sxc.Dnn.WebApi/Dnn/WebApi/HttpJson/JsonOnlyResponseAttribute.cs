using System.Linq;
using System.Net.Http.Formatting;
using System.Text.Json;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using ToSic.Eav.Serialization;
using ToSic.Eav.WebApi.Serialization;
using ToSic.Sxc.WebApi;

// Special case: this should enforce json formatting
// It's only needed in .net4x where the default is xml
namespace ToSic.Sxc.Dnn.WebApi.HttpJson
{
    public class JsonOnlyResponseAttribute : ActionFilterAttribute, IControllerConfiguration
    {
       
        public void Initialize(HttpControllerSettings controllerSettings, HttpControllerDescriptor controllerDescriptor)
        {
            var formatters = controllerSettings.Formatters;

            // Remove the XML formatter
            formatters.Remove(controllerSettings.Formatters.XmlFormatter);
            
            // For older apis we need to leave NewtonsoftJson
            if (GetCustomAttributes(controllerDescriptor.ControllerType).OfType<DefaultToNewtonsoftForHttpJsonAttribute>().Any())
            {
                // TODO: IF IT ALSO has a JsonFormatter, don't exit - so v14 can also work
                return;
            }

            // For newer apis we need to use System.Text.Json, but generated per request
            // because of DI dependencies for EavJsonConvertors in new generated JsonOptions

            // Remove default JsonMediaTypeFormatter (Newtonsoft)
            formatters.OfType<JsonMediaTypeFormatter>().ToList()
                .ForEach(f => controllerSettings.Formatters.Remove(f));

            // Get JsonFormatterAttribute from controller
            var jsonFormatterAttribute = GetCustomAttributes(controllerDescriptor.ControllerType).OfType<JsonFormatterAttribute>().FirstOrDefault();

            // Add SystemTextJsonMediaTypeFormatter with JsonSerializerOptions based on JsonFormatterAttribute from controller
            if (!formatters.OfType<SystemTextJsonMediaTypeFormatter>().Any())
                formatters.Insert(0, SystemTextJsonMediaTypeFormatterFactory(jsonFormatterAttribute, controllerDescriptor));
        }

        public override void OnActionExecuting(HttpActionContext context)
        {
            var controllerDescriptor = context.ControllerContext.ControllerDescriptor;

            // For older apis we need to leave
            if (GetCustomAttributes(controllerDescriptor.ControllerType).OfType<DefaultToNewtonsoftForHttpJsonAttribute>().Any())
            {
                // TODO: IF IT ALSO has a JsonFormatter, don't exit - so v14 can also work
                return;
            }

            // Get JsonFormatterAttribute from action method
            var jsonFormatterAttributeOnAction = context?.ActionDescriptor.GetCustomAttributes<JsonFormatterAttribute>().FirstOrDefault();

            // Nothing to do when JsonFormatterAttribute is missing on action method
            if (jsonFormatterAttributeOnAction == null)
                return;

            var formatters = context.ControllerContext.ControllerDescriptor.Configuration.Formatters;

            // Remove default SystemTextJsonMediaTypeFormatter with JsonSerializerOptions based on JsonFormatterAttribute from controller
            formatters.OfType<SystemTextJsonMediaTypeFormatter>().ToList()
                .ForEach(f => formatters.Remove(f));

            // Add SystemTextJson JsonMediaTypeFormatter with JsonSerializerOptions based on JsonFormatterAttribute from action method
            if (!formatters.OfType<SystemTextJsonMediaTypeFormatter>().Any())
                formatters.Insert(0, SystemTextJsonMediaTypeFormatterFactory(jsonFormatterAttributeOnAction, controllerDescriptor, context));
        }

        private SystemTextJsonMediaTypeFormatter SystemTextJsonMediaTypeFormatterFactory(JsonFormatterAttribute jsonFormatterAttribute, HttpControllerDescriptor controllerDescriptor, HttpActionContext context = null)
        {
            // Build Eav to Json converters for api v15
            var eavJsonConverterFactory = (jsonFormatterAttribute?.AutoConvertEntity == false) ? null : 
                controllerDescriptor.Configuration.DependencyResolver.GetService(typeof(EavJsonConverterFactory)) as EavJsonConverterFactory;

            var jsonSerializerOptions = JsonOptions.UnsafeJsonWithoutEncodingHtmlOptionsFactory(eavJsonConverterFactory);

            if (jsonFormatterAttribute?.Casing == Casing.CamelCase)
            {
                jsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                jsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            }

            return new SystemTextJsonMediaTypeFormatter { JsonSerializerOptions = jsonSerializerOptions };
        }
    }
}

