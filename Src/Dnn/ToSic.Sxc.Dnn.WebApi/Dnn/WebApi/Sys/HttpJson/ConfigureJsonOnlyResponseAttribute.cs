using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace ToSic.Sxc.Dnn.WebApi.Sys.HttpJson;

/// <summary>
/// Configure the WebApi controller to only use JSON responses.
/// This should enforce JSON formatting.
/// It's only needed in .net4x where the default is xml.
/// </summary>
/// <remarks>
/// The proper way to implement per-controller configuration is to create a custom attribute that implements IControllerConfiguration and
/// modifies the HttpControllerSettings (not the global configuration directly)
/// </remarks>
[ShowApiWhenReleased(ShowApiMode.Never)]   // unclear if this needs to be public
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public class ConfigureJsonOnlyResponseAttribute : ActionFilterAttribute, IControllerConfiguration, IHasLog
{
    // For debugging

    // turn on if we ever need to debug stuff again, but avoid this in production, unless really, really necessaryE
#if DEBUG
    private static readonly bool LogAnything = true;
#else
    private static readonly bool LogAnything = false; 
#endif

    // ReSharper disable ConvertToConstant.Local
    private static readonly bool LogDetails = false;
    private static readonly bool TestThrowInitialize = false;
    private static readonly bool TestThrowOnActionExecuting = false;
    // ReSharper restore ConvertToConstant.Local

    public ILog Log { get; } = LogAnything ? new Log("Api.JsnAttr") : null;

    private bool IsDebugEnabled()
        => new GlobalDebugParser(LogDetails ? Log : null).IsDebugEnabled();

    // Keys for per-request storage (avoid header mutation)
    private const string DebugFlagKey = "2sxc.DebugEnabled";

    /// <summary>
    /// This will just run once for every controller.
    /// </summary>
    /// <param name="controllerSettings"></param>
    /// <param name="controllerDescriptor"></param>
    /// <remarks>
    /// The proper way to implement per-controller configuration is to modifies the HttpControllerSettings (not the global configuration directly)
    /// This approach creates a controller-specific configuration that is isolated from the global settings.
    /// https://learn.microsoft.com/en-us/aspnet/web-api/overview/advanced/configuring-aspnet-web-api#per-controller-configuration
    /// </remarks>
    public void Initialize(HttpControllerSettings controllerSettings, HttpControllerDescriptor controllerDescriptor)
    {
        // Create an independent log for this operation - don't use a class-level log because it would grow too much
        // add to insights-history for analytic
        PlaceLogInHistory(Log);
        var l = Log.Fn($"{nameof(controllerDescriptor)}: {controllerDescriptor.ControllerType.FullName}");

        try
        {
            var debugEnabled = IsDebugEnabled(); // no request context here

            if (debugEnabled)
                DnnJsonFormattersDebug.DumpFormattersToLog(Log, "init-before", controllerSettings.Formatters);

            // This creates a controller-specific configuration
            var formattersManager = new DnnJsonFormattersManager(Log);
            formattersManager.ReconfigureControllerWithBestSerializers(
                controllerSettings.Formatters,
                GetCustomAttributes(controllerDescriptor.ControllerType)
            );

            if (debugEnabled)
                DnnJsonFormattersDebug.DumpFormattersToLog(Log, "init-after", controllerSettings.Formatters);

            // Test throwing an exception. Usually in debugging you will move the execution cursor to here to generate errors for specific requests
            if (TestThrowInitialize)
                throw new($"Test Exception in {nameof(Initialize)}");

        }
        catch (Exception ex)
        {
            // Add info - and add to system log as we want to see what actually happened
            l.Ex(ex);
            throw;
        }
        finally
        {
            l.Done();
        }
    }

    /// <summary>
    /// Dynamic Per-Request Configuration
    /// </summary>
    /// <param name="context"></param>
    public override void OnActionExecuting(HttpActionContext context)
    {
        // Log = new Log("Dnn.Attr"); // new log for each run
        PlaceLogInHistory(Log);
        var l = Log.Fn($"{nameof(context.Request.RequestUri)}:{context.Request?.RequestUri}");

        try
        {
            // Ensure we only configure once per request (probably not necessary anymore)
            if (PerRequestConfigurationHelper.SkipOnMultipleExecutionsOnTheSameRequest(context, l))
            {
                l.Done("exit early, repeat on same request");
                return; 
            }

            var requestProperties = context.Request?.Properties;

            // Cache debug-flag per request
            var debugEnabled = GetCachedDebugEnabled(requestProperties);

            var oldFormatters = context.ControllerContext.ControllerDescriptor.Configuration.Formatters;
            if (oldFormatters == null)
            {
                l.Done("formatters are null, unexpected");
                return;
            }

            if (debugEnabled)
                DnnJsonFormattersDebug.DumpFormattersToLog(Log, "action-before", oldFormatters);

            // Re-use manager instance per request if possible to avoid repeated allocations
            var dnnJsonFormattersManager = new DnnJsonFormattersManager(Log);

            var jsonFormatterAttributeOnAction = context.ActionDescriptor
                .GetCustomAttributes<JsonFormatterAttribute>()
                .FirstOrDefault();

            var perRequestConfiguration = PerRequestConfigurationHelper
                .CreateAndApplyPerRequestConfiguration(context, dnnJsonFormattersManager, jsonFormatterAttributeOnAction);

            if (debugEnabled && perRequestConfiguration != null)
                DnnJsonFormattersDebug.DumpFormattersToLog(Log, "action-after", perRequestConfiguration.Formatters);

            l.Done();

            // Test throwing an exception. Usually in debugging you will move the execution cursor to here to generate errors for specific requests
            if (TestThrowOnActionExecuting)
                throw new($"Test Exception in {nameof(OnActionExecuting)}");
        }
        catch (Exception ex)
        {
            // Add info - and add to system log as we want to see what actually happened
            l.Ex(ex);
            throw;
        }
    }

    // Cache debug-flag per request
    private bool GetCachedDebugEnabled(IDictionary<string, object> requestProperties)
    {
        if (requestProperties?.TryGetTyped(DebugFlagKey, out bool dbgBool) == true)
            return dbgBool;

        var debugEnabled = IsDebugEnabled();
        if (requestProperties != null)
            requestProperties[DebugFlagKey] = debugEnabled;

        return debugEnabled;
    }


    /// <summary>
    /// since it's really hard to debug attribute/serialization issues, try to log this problem
    /// </summary>
    private void PlaceLogInHistory(ILog log)
    {
        if (LogAnything)
            GetService<ILogStore>()?.Add("webapi-serialization", log);
    }

    // Do NOT CACHE the ServiceProvider, as this attribute seems to be long-lived and shared between requests
    private TService GetService<TService>() where TService : class
        => DnnStaticDi.GetPageScopedServiceProvider().Build<TService>(Log);
}

