using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace ToSic.Sxc.Dnn.WebApi.Sys.HttpJson;

/// <summary>
/// Configure the WebApi controller to only use JSON responses.
/// This should enforce JSON formatting.
/// It's only needed in .net4x where the default is xml.
/// </summary>
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

    public override void OnActionExecuting(HttpActionContext context)
    {
        // Log = new Log("Dnn.Attr"); // new log for each run
        PlaceLogInHistory(Log);
        var l = Log.Fn($"{nameof(context.Request.RequestUri)}:{context.Request?.RequestUri}");

        try
        {
            var request = context.Request;
            var props = request?.Properties;

            // Cache debug-flag per request
            bool debugEnabled;
            if (props != null && props.TryGetValue(DebugFlagKey, out var dbgObj) && dbgObj is bool dbgBool)
                debugEnabled = dbgBool;
            else
            {
                debugEnabled = IsDebugEnabled();
                if (props != null)
                    props[DebugFlagKey] = debugEnabled;
            }

            if (debugEnabled)
                DnnJsonFormattersManager.DumpFormattersToLog(Log, "action-before", context.ControllerContext.ControllerDescriptor.Configuration.Formatters);

            if (SkipOnMultipleExecutionsOnTheSameRequest(context, l))
                return; // ensure we only configure once per request

            // Re-use manager instance per request if possible to avoid repeated allocations
            var mgr = props != null && props.TryGetValue(ManagerKey, out var mgrObj) && mgrObj is DnnJsonFormattersManager existing
                ? existing
                : new DnnJsonFormattersManager(Log);
            if (props != null && !props.ContainsKey(ManagerKey))
                props[ManagerKey] = mgr;

            mgr.ReconfigureActionWithContextAwareSerializer(
                context.ControllerContext.ControllerDescriptor,
                context.ActionDescriptor
                    .GetCustomAttributes<JsonFormatterAttribute>()
                    .FirstOrDefault()
            );

            if (debugEnabled)
                DnnJsonFormattersManager.DumpFormattersToLog(Log, "action-after", context.ControllerContext.ControllerDescriptor.Configuration.Formatters);

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
}