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
        => new DebugRequestParser(LogDetails ? Log : null).IsDebugEnabled();

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
            if (IsDebugEnabled())
                DnnJsonFormattersManager.DumpFormattersToLog(Log, "init-before", controllerSettings.Formatters);

            var formattersManager = new DnnJsonFormattersManager(Log);
            formattersManager.ReconfigureControllerWithBestSerializers(
                controllerSettings.Formatters,
                GetCustomAttributes(controllerDescriptor.ControllerType)
            );

            if (IsDebugEnabled())
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
        PlaceLogInHistory(Log);
        var l = Log.Fn($"{nameof(context.Request.RequestUri)}:{context.Request?.RequestUri}");

        try
        {
            if (IsDebugEnabled())
                DnnJsonFormattersManager.DumpFormattersToLog(Log, "action-before", context.ControllerContext.ControllerDescriptor.Configuration.Formatters);

            if (SkipOnMultipleExecutionsOnTheSameRequest(context, l))
                return; // ensure we only configure once per request

            var formattersManager = new DnnJsonFormattersManager(Log);
            formattersManager.ReconfigureActionWithContextAwareSerializer(
                context.ControllerContext.ControllerDescriptor,
                context.ActionDescriptor
                    .GetCustomAttributes<JsonFormatterAttribute>()
                    .FirstOrDefault()
            );

            if (IsDebugEnabled())
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
        // Mark request so we can detect multiple executions on the same request and skip reconfiguration
        const string markerHeader = "X-2sxc-JsonFormatter-Configured";
        var headers = context.Request?.Headers;
        if (headers != null)
        {
            if (headers.Contains(markerHeader))
            {
                // increase marker value with count - for debugging
                var existingCount = 0;
                var existingValue = headers.GetValues(markerHeader)?.FirstOrDefault();
                if (!string.IsNullOrEmpty(existingValue) && int.TryParse(existingValue, out var parsed))
                    existingCount = parsed;
                existingCount++;

                headers.Remove(markerHeader);
                headers.Add(markerHeader, existingCount.ToString());

                l.A($"Formatter configuration already ran for this request - skipping (count:{existingCount})");
                return true;
            }
            headers.Add(markerHeader, "1");
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

    public ILog Log => field ??= new Log("Dnn.Attr");

    private TService GetService<TService>() where TService : class
        => _serviceProvider.Get(DnnStaticDi.GetPageScopedServiceProvider).Build<TService>(Log);
    // Must cache it, to be really sure we use the same ServiceProvider in the same request
    private readonly GetOnce<IServiceProvider> _serviceProvider = new();
}