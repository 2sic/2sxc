using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using ToSic.Sxc.Dnn.Run;

namespace ToSic.Sxc.Dnn.WebApi.Sys;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class DnnLogWebApi : ActionFilterAttribute
{
    public override bool AllowMultiple => false;

    private const string AlreadyLogged = "LogDetailsAlreadyHappened";

    private const string LocalTimerKey = "WebApiCallTimer";

    public override void OnActionExecuting(HttpActionContext actionContext)
    {
        try
        {
            // Set up a log timer showing the current URL to wrap the entire request
            var reqProps = actionContext.Request.Properties;
            reqProps.TryGetTyped(LocalTimerKey, out ILogCall wrappingCall);
            // Avoid duplicate start/stop
            if (wrappingCall == null)
            {
                var x = actionContext.ControllerContext.Controller;
                var hasLog = x as IHasLog;
                wrappingCall = (hasLog?.Log).Fn(
                    "api: " + actionContext.Request?.RequestUri?.AbsoluteUri,
                    timer: true,
                    // ReSharper disable once ExplicitCallerInfoArgument
                    cName: "Api"
                );

                // Remember it, so we can include the stop-information
                reqProps[LocalTimerKey] = wrappingCall;
            }
        }
        catch { /* ignore */ }

        base.OnActionExecuting(actionContext);
    }


    public override void OnActionExecuted(HttpActionExecutedContext actionContext)
    {
        base.OnActionExecuted(actionContext);

        try
        {
            var reqProps = actionContext.Request.Properties;

            // check if already logged, and set property to prevent double-logging
            if (reqProps.ContainsKey(AlreadyLogged))
                return;
            reqProps.Add(AlreadyLogged, true);

            if (reqProps.TryGetTyped(LocalTimerKey, out ILogCall wrappingCall))
                wrappingCall.Done();

            // check if we have any logging details for this request
            if (!reqProps.TryGetTyped(EavLogKey, out LogStoreEntry logStoreEntry))
                return;

            // check if we have additional context information (portal, module, etc.)
            reqProps.TryGetValue(DnnContextKey, out var dnnContext);

            DnnLogging.LogToDnn("2sxc-Api", 
                actionContext.Request.RequestUri.PathAndQuery,
                logStoreEntry.Log, 
                dnnContext as IDnnContext);
        }
        catch
        {
            DnnLogging.TryToReportLoggingFailure("WebApiLogDetails");
        }
    }

}