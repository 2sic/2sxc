using System.Net.Http.Formatting;
using System.Web.Http.Controllers;
using ToSic.Sxc.WebApi.Sys.ActionFilters;

namespace ToSic.Sxc.Dnn.WebApi.Sys.HttpJson;
internal class DnnJsonFormattersManager(ILog parentLog): HelperBase(parentLog, "Dnn.JFManager")
{
    // For debugging
    // ReSharper disable ConvertToConstant.Local
    private static readonly bool LogDetails = false;
    // ReSharper restore ConvertToConstant.Local

    /// <summary>
    /// Name used in the Tracer wrapper around SystemTextJsonMediaTypeFormatter
    /// </summary>
    private const string SystemTextJsonMediaTypeFormatterName = "System.Net.Http.Formatting.SystemTextJsonMediaTypeFormatter";

    private bool IsTraceEnabled()
        => new DebugRequestParser(LogDetails ? Log : null).IsDebugEnabled();

    internal static void DumpFormattersToLog(ILog log, string phase, MediaTypeFormatterCollection formatters)
    {
        var l = log.Fn($"dump:{phase}");
        try
        {
            l.A($"Dumping formatters ({formatters?.Count ?? -1}) at {phase}");
            if (formatters == null)
            {
                l.A("Formatters collection is NULL");
                return;
            }
            for (var i = 0; i < formatters.Count; i++)
            {
                var f = formatters[i];
                if (f == null)
                {
                    l.A($"[{i}] NULL formatter");
                    continue;
                }
                var type = f.GetType();
                var info = $"[{i}] {type.FullName}";
                // Try unwrap common tracer pattern
                try
                {
                    var innerProp = type.GetProperty("InnerFormatter");
                    if (innerProp != null)
                    {
                        var inner = innerProp.GetValue(f) as MediaTypeFormatter;
                        if (inner != null)
                        {
                            info += $" -> inner: {inner.GetType().FullName}";
                        }
                        else
                        {
                            info += " -> inner: null";
                        }
                    }
                }
                catch { /* ignore reflection errors */ }
                l.A(info);
            }
        }
        catch (Exception ex)
        {
            l.Ex(ex);
        }
        finally
        {
            l.Done();
        }
    }

    private void EnsureNoNulls(MediaTypeFormatterCollection formatters)
    {
        var l = Log.Fn();
        try
        {
            if (formatters == null)
            {
                l.A("Formatters is null - nothing to sanitize");
                return;
            }

            if (formatters.All(f => f != null))
            {
                l.A("No null formatters found");
                return;
            }

            l.A("Null formatter(s) detected - will rebuild collection without nulls");
            var safe = formatters
                .Where(f => f != null)
                .ToList();

            formatters.Clear();

            foreach (var f in safe)
                formatters.Add(f);

            l.A($"Sanitized formatters count: {formatters.Count}");
        }
        catch (Exception ex)
        {
            l.Ex(ex);
        }
        finally
        {
            l.Done();
        }
    }

    internal void ReconfigureControllerWithBestSerializers(MediaTypeFormatterCollection formatters, Attribute[] customAttributes)
    {
        var l = Log.Fn();

        if (IsTraceEnabled())
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

            if (IsTraceEnabled())
                DumpFormattersToLog(Log, "after-controller-skip", formatters);

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
            try
            {
                formatters.Remove(f);
            }
            catch (Exception ex)
            {
                l.E("Newtonsoft remove failed");
                l.Ex(ex);
            }

        // Add SystemTextJsonMediaTypeFormatter with JsonSerializerOptions based on JsonFormatterAttribute from controller
        ReplaceJsonFormatterWithNewInstance(formatters, jsonFormatterAttribute);

        EnsureNoNulls(formatters);

        if (IsTraceEnabled())
            DumpFormattersToLog(Log, "after-controller", formatters);

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
        
        if (IsTraceEnabled())
            DumpFormattersToLog(Log, "before-action", formatters);

        ReplaceJsonFormatterWithNewInstance(formatters, jsonFormatterAttributeOnAction);

        EnsureNoNulls(formatters);

        if (IsTraceEnabled())
            DumpFormattersToLog(Log, "after-action", formatters);

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
            try
            {
                formatters.Remove(f);
            }
            catch (Exception ex)
            {
                l.E("Remove SystemTextJson failed");
                l.Ex(ex);
            }

        // Tracers seem to be wrapped formatters which can also do trace-logging.
        // We noticed that these are prepared
        var tracersToRemove = formattersListCopy
            .Where(f => f?.ToString() == SystemTextJsonMediaTypeFormatterName)
            .ToList();

        l.A($"Will remove {tracersToRemove.Count} of type {nameof(MediaTypeFormatter)}");
        foreach (var f in tracersToRemove)
            try
            {
                formatters.Remove(f);
            }
            catch (Exception ex)
            {
                l.E("Remove tracer failed");
                l.Ex(ex);
            }

        // Unwrap tracers to get inner SystemTextJsonMediaTypeFormatter instances using reflection
        var unwrappedFormatters = tracersToRemove
            .Select(tracer =>
            {
                try
                {
                    // MediaTypeFormatterTracer has an InnerFormatter property that contains the actual formatter
                    var innerFormatterProperty = tracer.GetType().GetProperty("InnerFormatter");
                    return innerFormatterProperty?.GetValue(tracer) as SystemTextJsonMediaTypeFormatter;
                }
                catch (Exception ex)
                {
                    l.E("Failed to unwrap tracer");
                    l.Ex(ex);
                    return null;
                }
            })
            .Where(f => f != null)
            .ToList();

        l.A($"Unwrapped {unwrappedFormatters.Count} {nameof(SystemTextJsonMediaTypeFormatter)} from tracers");
        formattersToRemove.AddRange(unwrappedFormatters);

        // Add SystemTextJson JsonMediaTypeFormatter with JsonSerializerOptions based on JsonFormatterAttribute from action method
        if (!formatters.OfType<SystemTextJsonMediaTypeFormatter>().Any())
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
                    () => JsonFormatterCasingHelpersForDnn.ExtractCasingFromFormatters(formattersToRemove),
                    jsonSerializerOptions => new SystemTextJsonMediaTypeFormatter { JsonSerializerOptions = jsonSerializerOptions }
                );
            }
            catch (Exception ex)
            {
                l.E("CreateNewFormatterFactory threw");
                l.Ex(ex);
            }

            if (newFactory == null)
            {
                l.A("CreateNewFormatterFactory returned NULL - will not insert");
            }
            else
            {
                try
                {
                    formatters.Insert(0, newFactory);
                }
                catch (Exception ex)
                {
                    l.E("Insert new formatter failed");
                    l.Ex(ex);
                }
            }
        }
        else
            l.A($"It has a {nameof(SystemTextJsonMediaTypeFormatter)}, so won't add.");

        l.Done();
    }

}
