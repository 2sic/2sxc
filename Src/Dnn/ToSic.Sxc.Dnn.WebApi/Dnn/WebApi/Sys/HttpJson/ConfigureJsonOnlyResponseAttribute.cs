using System.Reflection;
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
    // ReSharper disable ConvertToConstant.Local
    private static readonly bool LogDetails = false;
    private static readonly bool TestThrowInitialize = false;
    private static readonly bool TestThrowOnActionExecuting = false;
    // ReSharper restore ConvertToConstant.Local
    
    private bool IsDebugEnabled()
        => new GlobalDebugParser(LogDetails ? Log : null).IsDebugEnabled();

    // Keys for per-request storage (avoid header mutation)
    private const string MarkerKey = "2sxc.JsonFormatter.Configured";
    private const string DebugFlagKey = "2sxc.DebugEnabled";
    private const string ManagerKey = "2sxc.JsonFormatter.Manager";

    /// <summary>
    /// This will just run once - I think for every controller...
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
        var l = Log.Fn($"{nameof(controllerDescriptor)}:{controllerDescriptor.ControllerType.FullName}");

        try
        {
            var debugEnabled = IsDebugEnabled(); // no request context here

            if (debugEnabled)
                DnnJsonFormattersManager.DumpFormattersToLog(Log, "init-before", controllerSettings.Formatters);

            // This creates a controller-specific configuration
            var formattersManager = new DnnJsonFormattersManager(Log);
            formattersManager.ReconfigureControllerWithBestSerializers(
                controllerSettings.Formatters,
                GetCustomAttributes(controllerDescriptor.ControllerType)
            );

            if (debugEnabled)
                DnnJsonFormattersManager.DumpFormattersToLog(Log, "init-after", controllerSettings.Formatters);

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
            var requestProperties = context.Request?.Properties;

            // Cache debug-flag per request
            var debugEnabled = GetCachedDebugEnabled(requestProperties);

            if (debugEnabled)
                DnnJsonFormattersManager.DumpFormattersToLog(Log, "action-before", context.ControllerContext.ControllerDescriptor.Configuration.Formatters);

            if (SkipOnMultipleExecutionsOnTheSameRequest(context, l))
                return; // ensure we only configure once per request

            // Re-use manager instance per request if possible to avoid repeated allocations
            var dnnJsonFormattersManager = GetCachedDnnJsonFormattersManager(requestProperties);

            var jsonFormatterAttributeOnAction = context.ActionDescriptor
                .GetCustomAttributes<JsonFormatterAttribute>()
                .FirstOrDefault();

            var perRequestConfiguration = CreatePerRequestConfiguration(context, dnnJsonFormattersManager, jsonFormatterAttributeOnAction);

            ApplyPerRequestConfiguration(context, perRequestConfiguration);

            if (debugEnabled && perRequestConfiguration != null)
                DnnJsonFormattersManager.DumpFormattersToLog(Log, "action-after", perRequestConfiguration.Formatters);

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
        finally
        {
            l.Done();
        }
    }

    // Cache debug-flag per request
    private bool GetCachedDebugEnabled(IDictionary<string, object> requestProperties)
    {
        bool debugEnabled;
        if (requestProperties?.TryGetValue(DebugFlagKey, out var dbgObj) == true
            && dbgObj is bool dbgBool)
        {
            debugEnabled = dbgBool;
        }
        else
        {
            debugEnabled = IsDebugEnabled();
            if (requestProperties != null)
                requestProperties[DebugFlagKey] = debugEnabled;
        }

        return debugEnabled;
    }

    // Re-use manager instance per request if possible to avoid repeated allocations
    private DnnJsonFormattersManager GetCachedDnnJsonFormattersManager(IDictionary<string, object> requestProperties)
    {
        var dnnJsonFormattersManager =
            requestProperties?.TryGetValue(ManagerKey, out var mgrObj) == true
            && mgrObj is DnnJsonFormattersManager existing
                ? existing
                : new DnnJsonFormattersManager(Log);
        if (requestProperties != null && !requestProperties.ContainsKey(ManagerKey))
            requestProperties[ManagerKey] = dnnJsonFormattersManager;
        return dnnJsonFormattersManager;
    }

    // Ensure we only configure once per request
    private static bool SkipOnMultipleExecutionsOnTheSameRequest(HttpActionContext context, ILogCall l)
    {
        // Use Request.Properties to mark rather than mutating headers (faster & avoids client-side confusion)
        var props = context.Request?.Properties;
        if (props != null)
        {
            if (props.TryGetValue(MarkerKey, out var existing) && existing is int cnt)
            {
                cnt++;
                props[MarkerKey] = cnt;
                l.A($"Formatter configuration already ran for this request - skipping (count:{cnt})");
                return true;
            }
            props[MarkerKey] = 1; // first time
        }
        return false;
    }

    /// <summary>
    /// since it's really hard to debug attribute/serialization issues, try to log this problem
    /// </summary>
    private void PlaceLogInHistory(ILog log)
    {
        _logStore ??= GetService<ILogStore>();
        _logStore?.Add("webapi-serialization", log);
    }
    private ILogStore _logStore;

    public ILog Log { get; private set; } = new Log("Dnn.Attr");

    private TService GetService<TService>() where TService : class
        => _serviceProvider.Get(DnnStaticDi.GetPageScopedServiceProvider).Build<TService>(Log);
    // Must cache it, to be really sure we use the same ServiceProvider in the same request
    private readonly GetOnce<IServiceProvider> _serviceProvider = new();

    private static HttpConfiguration CreatePerRequestConfiguration(
        HttpActionContext context,
        DnnJsonFormattersManager manager,
        JsonFormatterAttribute? jsonFormatterAttributeOnAction)
    {
        var originalConfig = context.ControllerContext.Configuration
            ?? throw new InvalidOperationException("Controller configuration is not available.");

        var controllerSettings = new HttpControllerSettings(originalConfig);

        manager.ReconfigureActionWithContextAwareSerializer(
            context.ControllerContext.ControllerDescriptor,
            controllerSettings.Formatters,
            jsonFormatterAttributeOnAction);

        return ApplyControllerSettingsAccessor.Value(controllerSettings, originalConfig);
    }

    // Cached reflection bridge to Web API’s internal HttpConfiguration.ApplyControllerSettings
    // so we can spin up per-request clones of the controller configuration without touching shared state 
    private static readonly Lazy<Func<HttpControllerSettings, HttpConfiguration, HttpConfiguration>> ApplyControllerSettingsAccessor
        = new(() =>
        {
            var method = typeof(HttpConfiguration)
                .GetMethod("ApplyControllerSettings", BindingFlags.NonPublic | BindingFlags.Static);
            if (method == null)
                throw new InvalidOperationException("Unable to locate HttpConfiguration.ApplyControllerSettings");
            return (Func<HttpControllerSettings, HttpConfiguration, HttpConfiguration>)method
                .CreateDelegate(typeof(Func<HttpControllerSettings, HttpConfiguration, HttpConfiguration>));
        });

    private static void ApplyPerRequestConfiguration(HttpActionContext context, HttpConfiguration configuration)
    {
        if (configuration == null)
            return;
        context.ControllerContext.Configuration = configuration;
        context.Request?.SetConfiguration(configuration);
    }
}
