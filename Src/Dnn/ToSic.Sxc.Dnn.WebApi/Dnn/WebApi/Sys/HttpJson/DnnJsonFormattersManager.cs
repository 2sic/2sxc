using System.Net.Http.Formatting;
using System.Web.Http.Controllers;
using ToSic.Sxc.WebApi.Sys.ActionFilters;
using static ToSic.Sxc.Dnn.WebApi.Sys.HttpJson.DnnJsonFormattersDebug;

namespace ToSic.Sxc.Dnn.WebApi.Sys.HttpJson;
internal class DnnJsonFormattersManager(ILog parentLog): HelperBase(parentLog, "Dnn.JFManager")
{
    // For debugging
    // ReSharper disable once ConvertToConstant.Local
    private static readonly bool LogDetails = false;

    /// <summary>
    /// Name used in the Tracer wrapper around SystemTextJsonMediaTypeFormatter
    /// </summary>
    private const string SystemTextJsonMediaTypeFormatterName = "System.Net.Http.Formatting.SystemTextJsonMediaTypeFormatter";

    private bool IsDebugEnabled()
        => new GlobalDebugParser(LogDetails ? Log : null).IsDebugEnabled();


    private object EnsureNoNulls(MediaTypeFormatterCollection formatters)
    {
        var l = Log.Fn<object>();
        try
        {
            if (formatters == null)
                return l.ReturnNull("Formatters is null - nothing to sanitize");

            if (formatters.All(f => f != null))
                return l.ReturnNull("No null formatters found");

            l.A("Null formatter(s) detected - will rebuild collection without nulls");
            var safe = formatters
                .Where(f => f != null)
                .ToList();

            formatters.Clear();

            foreach (var f in safe)
                formatters.Add(f);

            return l.ReturnNull($"Sanitized formatters count: {formatters.Count}");
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            return l.ReturnNull("Exception during EnsureNoNulls");
        }
        finally
        {
            l.Done();
        }
    }

    // This creates a controller-specific configuration
    internal void ReconfigureControllerWithBestSerializers(MediaTypeFormatterCollection formatters, Attribute[] customAttributes)
    {
        var l = Log.Fn();

        if (IsDebugEnabled())
            DumpFormattersToLog(Log, "before-controller", formatters);

        // Remove the XML formatter
        l.A("Will remove the default XmlFormatter");
        try
        {
            if (formatters?.XmlFormatter != null)
                formatters.Remove(formatters.XmlFormatter);
        }
        catch (Exception ex)
        {
            l.E("XmlFormatter remove failed");
            l.Ex(ex);
        }

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

            if (IsDebugEnabled())
                DumpFormattersToLog(Log, "after-controller-skip", formatters);

            return;
        }

        // For newer apis we need to use System.Text.Json, but generated per request
        // because of DI dependencies for EavJsonConvertors in new generated JsonOptions

        // Remove default JsonMediaTypeFormatter (Newtonsoft) with a single rebuild pass
        try
        {
            var keep = new List<MediaTypeFormatter>(formatters.Count);
            foreach (var f in formatters)
                if (f is not JsonMediaTypeFormatter)
                    keep.Add(f);
            formatters.Clear();
            foreach (var f in keep)
                formatters.Add(f);
        }
        catch (Exception ex)
        {
            l.E("Newtonsoft remove (rebuild) failed");
            l.Ex(ex);
        }

        // Add SystemTextJsonMediaTypeFormatter with JsonSerializerOptions based on JsonFormatterAttribute from controller
        ReplaceJsonFormatterWithNewInstance(formatters, jsonFormatterAttribute);

        EnsureNoNulls(formatters);

        if (IsDebugEnabled())
            DumpFormattersToLog(Log, "after-controller", formatters);

        l.Done();
    }

    internal void ReconfigureActionWithContextAwareSerializer(
        HttpControllerDescriptor controllerDescriptor,
        MediaTypeFormatterCollection formatters,
        JsonFormatterAttribute? jsonFormatterAttributeOnAction)
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

        if (formatters == null)
        {
            l.Done("Formatters collection is null - nothing to reconfigure");
            return;
        }

        l.A($"Found {formatters.Count} formatters");

        if (IsDebugEnabled())
            DumpFormattersToLog(Log, "before-action", formatters);

        ReplaceJsonFormatterWithNewInstance(formatters, jsonFormatterAttributeOnAction);

        EnsureNoNulls(formatters);

        if (IsDebugEnabled())
            DumpFormattersToLog(Log, "after-action", formatters);

        l.Done();
    }


    internal void ReplaceJsonFormatterWithNewInstance(MediaTypeFormatterCollection formatters, JsonFormatterAttribute jsonFormatterAttributeOnAction)
    {
        var l = Log.Fn();

        // 2025-11-08 2dm - creating a list to avoid multiple enumeration of formatters
        // since I sometimes observed a single error after restart "Collection was modified; enumeration operation may not execute."
        // Defensive: copy once to avoid collection-modified exceptions
        var formattersListCopy = formatters.ToList();

        // Collect formatters to remove (STJ and tracer-like) in one pass
        var toKeep = new List<MediaTypeFormatter>(formattersListCopy.Count);
        var removedForCasingMedia = new List<MediaTypeFormatter>(); // all removed items
        var removedStj = new List<SystemTextJsonMediaTypeFormatter>(); // only STJ for casing helper
        foreach (var f in formattersListCopy)
        {
            if (f is SystemTextJsonMediaTypeFormatter stj)
            {
                removedForCasingMedia.Add(stj);
                removedStj.Add(stj);
                continue;
            }
            
            // Tracers seem to be wrapped formatters which can also do trace-logging.
            // We noticed that these are prepared

            // Detect tracer by ToString match
            if (f != null && f.ToString() == SystemTextJsonMediaTypeFormatterName)
            {
                removedForCasingMedia.Add(f);
                // Try unwrap actual inner STJ if present
                try
                {
                    var innerProp = f.GetType().GetProperty("InnerFormatter");
                    var inner = innerProp?.GetValue(f) as SystemTextJsonMediaTypeFormatter;
                    if (inner != null)
                    {
                        removedForCasingMedia.Add(inner);
                        removedStj.Add(inner);
                    }
                }
                catch (Exception ex)
                {
                    l.E("Failed to unwrap tracer");
                    l.Ex(ex);
                }
                continue;
            }

            toKeep.Add(f);
        }

        // If no STJ present after removal, add new one configured by factory
        var hasStj = toKeep.Any(x => x is SystemTextJsonMediaTypeFormatter);
        if (!hasStj)
        {
            l.A($"Will add {nameof(SystemTextJsonMediaTypeFormatter)} since none were found");
            MediaTypeFormatter newFactory = null;
            try
            {
                newFactory = JsonConverterFactoryHelpers.CreateNewFormatterFactory(
                    // Get the service provider from the current request scope using DnnStaticDi helper
                    // This ensures all services (including EavJsonConverterFactory and its dependencies) use the current request's culture
                    DnnStaticDi.GetPageScopedServiceProvider(),
                    jsonFormatterAttributeOnAction,
                    () => JsonFormatterCasingHelpersForDnn.ExtractCasingFromFormatters(removedStj),
                    jsonSerializerOptions => new SystemTextJsonMediaTypeFormatter { JsonSerializerOptions = jsonSerializerOptions }
                );
            }
            catch (Exception ex)
            {
                l.E("CreateNewFormatterFactory threw");
                l.Ex(ex);
            }

            if (newFactory != null)
                toKeep.Insert(0, newFactory);
            else
                l.A("CreateNewFormatterFactory returned NULL - will not insert");
        }
        else
        {
            l.A($"It has a {nameof(SystemTextJsonMediaTypeFormatter)}, so won't add.");
        }

        // Rebuild collection once to avoid multiple Remove calls
        try
        {
            formatters.Clear();
            foreach (var f in toKeep)
                formatters.Add(f);
        }
        catch (Exception ex)
        {
            l.E("Rebuild formatters failed");
            l.Ex(ex);
        }

        l.Done();
    }

}
