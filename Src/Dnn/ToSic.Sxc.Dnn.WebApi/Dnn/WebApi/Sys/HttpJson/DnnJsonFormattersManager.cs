using System.Net.Http.Formatting;
using System.Web.Http.Controllers;
using ToSic.Sxc.WebApi.Sys.ActionFilters;

namespace ToSic.Sxc.Dnn.WebApi.Sys.HttpJson;
internal class DnnJsonFormattersManager(ILog parentLog): HelperBase(parentLog, "Dnn.JFManager")
{
    /// <summary>
    /// Name used in the Tracer wrapper around SystemTextJsonMediaTypeFormatter
    /// </summary>
    private const string SystemTextJsonMediaTypeFormatterName = "System.Net.Http.Formatting.SystemTextJsonMediaTypeFormatter";

    internal void ReconfigureControllerWithBestSerializers(MediaTypeFormatterCollection formatters, Attribute[] customAttributes)
    {
        var l = Log.Fn();

        // Remove the XML formatter
        l.A("Will remove the default XmlFormatter");
        formatters.Remove(formatters.XmlFormatter);

        // Get JsonFormatterAttribute from controller - would mark the controller to use System.Text.Json
        var jsonFormatterAttribute = customAttributes
            .OfType<JsonFormatterAttribute>()
            .FirstOrDefault();

        // For older apis we need to leave NewtonsoftJson (when JsonFormatterAttribute is missing on controller)
        var keepOldNewtonsoft = customAttributes
            .OfType<DefaultToNewtonsoftForHttpJsonAttribute>()
            .Any();
        if (keepOldNewtonsoft && jsonFormatterAttribute == null)
        {
            l.Done($"Has {nameof(DefaultToNewtonsoftForHttpJsonAttribute)} and no custom {nameof(JsonFormatterAttribute)} will leave serializers intact.");
            return;
        }

        // For newer apis we need to use System.Text.Json, but generated per request
        // because of DI dependencies for EavJsonConvertors in new generated JsonOptions

        // Remove default JsonMediaTypeFormatter (Newtonsoft)
        var newtonSoftFormatters = formatters
            .OfType<JsonMediaTypeFormatter>()
            .ToList();
        l.A($"Will remove {newtonSoftFormatters.Count} NewtonSoft formatters");
        foreach (var f in newtonSoftFormatters)
            formatters.Remove(f);

        // Add SystemTextJsonMediaTypeFormatter with JsonSerializerOptions based on JsonFormatterAttribute from controller
        ReplaceJsonFormatterWithNewInstance(formatters, jsonFormatterAttribute);
        l.Done();
    }

    internal void ReconfigureActionWithContextAwareSerializer(HttpControllerDescriptor controllerDescriptor, JsonFormatterAttribute? jsonFormatterAttributeOnAction)
    {
        var l = Log.Fn();

        // Get JsonFormatterAttribute from action **Method**
        l.A(jsonFormatterAttributeOnAction == null
            ? $"{nameof(JsonFormatterAttribute)} is missing on action method." // note: before 20.09 it returned here
            : $"Method has custom {nameof(JsonFormatterAttribute)}");

        // For older apis we need to leave (when JsonFormatterAttribute is missing on action method)
        var controllerHasDefaultToOld = Attribute.GetCustomAttributes(controllerDescriptor.ControllerType)
            .OfType<DefaultToNewtonsoftForHttpJsonAttribute>()
            .Any();
        if (controllerHasDefaultToOld)
        {
            l.Done($"Controller has {nameof(DefaultToNewtonsoftForHttpJsonAttribute)}, will exit leaving old serializers.");
            return;
        }

        var formatters = controllerDescriptor.Configuration.Formatters;
        l.A($"Found {formatters.Count} formatters");

        ReplaceJsonFormatterWithNewInstance(formatters, jsonFormatterAttributeOnAction);

        l.Done();
    }


    internal void ReplaceJsonFormatterWithNewInstance(MediaTypeFormatterCollection formatters, JsonFormatterAttribute jsonFormatterAttributeOnAction)
    {
        var l = Log.Fn();

        // 2025-11-08 2dm - creating a list to avoid multiple enumeration of formatters
        // since I sometimes observed a single error after restart "Collection was modified; enumeration operation may not execute."
        var formattersListCopy = formatters.ToList();

        // Remove default SystemTextJsonMediaTypeFormatter with JsonSerializerOptions based on JsonFormatterAttribute from controller
        var formattersToRemove = formattersListCopy
            .OfType<SystemTextJsonMediaTypeFormatter>()
            .ToList();
        l.A($"Will remove {formattersToRemove.Count} of type {nameof(SystemTextJsonMediaTypeFormatter)}");
        foreach (var f in formattersToRemove)
            formatters.Remove(f);

        // Tracers seem to be wrapped formatters which can also do trace-logging.
        // We noticed that these are prepared
        var tracersToRemove = formattersListCopy
            .Where(f => f?.ToString() == SystemTextJsonMediaTypeFormatterName)
            .ToList();
        l.A($"Will remove {tracersToRemove.Count} of type {nameof(MediaTypeFormatter)}");
        foreach (var f in tracersToRemove)
            formatters.Remove(f);

        // Unwrap tracers to get inner SystemTextJsonMediaTypeFormatter instances using reflection
        var unwrappedFormatters = tracersToRemove
            .Select(tracer =>
            {
                // MediaTypeFormatterTracer has an InnerFormatter property that contains the actual formatter
                var innerFormatterProperty = tracer.GetType().GetProperty("InnerFormatter");
                return innerFormatterProperty?.GetValue(tracer) as SystemTextJsonMediaTypeFormatter;
            })
            .Where(f => f != null)
            .ToList();

        l.A($"Unwrapped {unwrappedFormatters.Count} {nameof(SystemTextJsonMediaTypeFormatter)} from tracers");
        formattersToRemove.AddRange(unwrappedFormatters);

        // Add SystemTextJson JsonMediaTypeFormatter with JsonSerializerOptions based on JsonFormatterAttribute from action method
        if (!formatters.OfType<SystemTextJsonMediaTypeFormatter>().Any())
        {
            l.A($"Will add {nameof(SystemTextJsonMediaTypeFormatter)} since none were found");
            var newFactory = JsonConverterFactoryHelpers.CreateNewFormatterFactory(
                // Get the service provider from the current request scope using DnnStaticDi helper
                // This ensures all services (including EavJsonConverterFactory and its dependencies) use the current request's culture
                DnnStaticDi.GetPageScopedServiceProvider(),
                jsonFormatterAttributeOnAction,
                () => JsonFormatterCasingHelpersForDnn.ExtractCasingFromFormatters(formattersToRemove),
                jsonSerializerOptions => new SystemTextJsonMediaTypeFormatter { JsonSerializerOptions = jsonSerializerOptions }
            );
            formatters.Insert(0, newFactory);
        }
        else
            l.A($"It has a {nameof(SystemTextJsonMediaTypeFormatter)}, so won't add.");

        l.Done();
    }

}
