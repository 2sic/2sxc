using System.Linq;
using System.Net.Http.Formatting;
using System.Web;
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
        private const string JsonFormatterAttributeKey = "JsonFormatterAttributeOnController";

        public void Initialize(HttpControllerSettings controllerSettings, HttpControllerDescriptor controllerDescriptor)
        {
            var formatters = controllerSettings.Formatters;

            // Remove the XML formatter
            formatters.Remove(controllerSettings.Formatters.XmlFormatter);

            // Get JsonFormatterAttribute from controller
            var jsonFormatterAttribute = GetCustomAttributes(controllerDescriptor.ControllerType).OfType<JsonFormatterAttribute>().FirstOrDefault();

            // For older apis we need to leave NewtonsoftJson (when JsonFormatterAttribute is missing on controller)
            if (GetCustomAttributes(controllerDescriptor.ControllerType).OfType<DefaultToNewtonsoftForHttpJsonAttribute>().Any() && jsonFormatterAttribute == null)
                return;

            // For newer apis we need to use System.Text.Json, but generated per request
            // because of DI dependencies for EavJsonConvertors in new generated JsonOptions

            // Remove default JsonMediaTypeFormatter (Newtonsoft)
            formatters.OfType<JsonMediaTypeFormatter>().ToList()
                .ForEach(f => controllerSettings.Formatters.Remove(f));

            // Add SystemTextJsonMediaTypeFormatter with JsonSerializerOptions based on JsonFormatterAttribute from controller
            if (!formatters.OfType<SystemTextJsonMediaTypeFormatter>().Any())
                formatters.Insert(0, SystemTextJsonMediaTypeFormatterFactory(jsonFormatterAttribute, controllerDescriptor));

            // Save JsonFormatterAttribute on controller in HttpContext.Items for later use in OnActionExecuting
            if (jsonFormatterAttribute != null)
                HttpContext.Current.Items[JsonFormatterAttributeKey] = jsonFormatterAttribute;
        }

        public override void OnActionExecuting(HttpActionContext context)
        {
            var controllerDescriptor = context.ControllerContext.ControllerDescriptor;

            // Get JsonFormatterAttribute from action method
            var jsonFormatterAttributeOnAction = context?.ActionDescriptor.GetCustomAttributes<JsonFormatterAttribute>().FirstOrDefault();

            // For older apis we need to leave (when JsonFormatterAttribute is missing on action method)
            if (GetCustomAttributes(controllerDescriptor.ControllerType).OfType<DefaultToNewtonsoftForHttpJsonAttribute>().Any() && jsonFormatterAttributeOnAction == null)
                return;

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
            var eavJsonConverterFactory = GetEavJsonConverterFactory(jsonFormatterAttribute?.EntityFormat ?? GetJsonFormatterAttribute()?.EntityFormat, controllerDescriptor);

            var jsonSerializerOptions = JsonOptions.UnsafeJsonWithoutEncodingHtmlOptionsFactory(eavJsonConverterFactory);

            JsonFormatterHelpers.SetCasing(jsonFormatterAttribute?.Casing ?? GetJsonFormatterAttribute()?.Casing ?? Casing.Unspecified, jsonSerializerOptions);

            return new SystemTextJsonMediaTypeFormatter { JsonSerializerOptions = jsonSerializerOptions };
        }

        private JsonFormatterAttribute GetJsonFormatterAttribute() => 
            HttpContext.Current.Items.Contains(JsonFormatterAttributeKey) ? HttpContext.Current.Items[JsonFormatterAttributeKey] as JsonFormatterAttribute : null;

        private static EavJsonConverterFactory GetEavJsonConverterFactory(EntityFormat? entityFormat, HttpControllerDescriptor controllerDescriptor)
        {
            switch (entityFormat)
            {
                case null:
                case EntityFormat.Light:
                    return controllerDescriptor.Configuration.DependencyResolver.GetService(typeof(EavJsonConverterFactory)) as EavJsonConverterFactory;
                case EntityFormat.None:
                default:
                    return null;
            }
        }

        //private static void SetCasing(Casing casing, JsonSerializerOptions jsonSerializerOptions)
        //{
        //    // TODO: @STV
        //    // 1. Make casing non-null in parameter - the caller must ensure that it's ?? Casing.Default or something
        //    // 2. Use HasFlag - see code sample
        //    // 3. Deduplicate - I believe ATM we have the identical code in DNN/Oqtane, that's bad - better move to static in Sxc.WebApi somewhere
        //    if (casing.HasFlag(Casing.Camel))
        //    {

        //    }


        //    if (casing == Casing.Default
        //        || (casing & Casing.Camel) == Casing.Camel
        //        //|| (casing & Casing.ObjectDefault) == Casing.ObjectDefault
        //        //|| (casing & Casing.ObjectCamel) == Casing.ObjectCamel
        //        )
        //        jsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

        //    if (casing == Casing.Default
        //        || (casing & Casing.Camel) == Casing.Camel
        //        || (casing & Casing.PropertyDefault) == Casing.PropertyDefault
        //        || (casing & Casing.PropertyCamel) == Casing.PropertyCamel)
        //        jsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;

        //    if ((casing & Casing.Pascal) == Casing.Pascal
        //        //|| (casing & Casing.ObjectPascal) == Casing.ObjectPascal
        //        )
        //        jsonSerializerOptions.PropertyNamingPolicy = null;

        //    if ((casing & Casing.Pascal) == Casing.Pascal
        //        || (casing & Casing.PropertyPascal) == Casing.PropertyPascal)
        //        jsonSerializerOptions.DictionaryKeyPolicy = null;
        //}
    }
}

