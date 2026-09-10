using System.Web.Http.Filters;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Web.Http.Controllers;
using ToSic.Sxc.Dnn.Run;

namespace ToSic.Sxc.Dnn.WebApi.Sys;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class DnnLogWebApi : ActionFilterAttribute
{
    private static readonly ActivitySource Activities = new("ToSic.2sxc.WebApi");

    public override bool AllowMultiple => false;

    private const string AlreadyLogged = "LogDetailsAlreadyHappened";
    private const string ExecutionScope = "2sxc.ILoggerExecutionScope";

    public override void OnActionExecuting(HttpActionContext actionContext)
    {
        base.OnActionExecuting(actionContext);

        if (actionContext.ControllerContext.Controller is not DnnSxcControllerRoot controller)
            return;
        var logger = controller.SysHlp.GetService<ILoggerFactory>().CreateLogger(MicrosoftLoggerEventSink.Category);
        var appId = actionContext.ActionArguments.TryGetValue("appId", out var value) && value is int id ? id : (int?)null;
        var operation = $"{actionContext.ControllerContext.ControllerDescriptor.ControllerName}.{actionContext.ActionDescriptor.ActionName}";
        var scope = logger.BeginExecution(controller.Log, Activities, operation, appId: appId);
        if (scope != null)
            actionContext.Request.Properties[ExecutionScope] = scope;
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
        finally
        {
            if (actionContext.Request.Properties.TryGetValue(ExecutionScope, out var scope))
                (scope as IDisposable)?.Dispose();
        }
    }

}
