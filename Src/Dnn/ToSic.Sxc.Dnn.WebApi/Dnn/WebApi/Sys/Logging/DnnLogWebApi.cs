using System.Web.Http.Filters;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using Microsoft.Extensions.Logging;
using System.Web.Http.Controllers;
using ToSic.Sxc.Dnn.Run;

namespace ToSic.Sxc.Dnn.WebApi.Sys;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class DnnLogWebApi : FilterAttribute, IActionFilter
{
    private static readonly ActivitySource Activities = new(LogExecution.ActivitySourceName, LogExecution.ActivitySourceVersion);

    public override bool AllowMultiple => false;

    private const string AlreadyLogged = "LogDetailsAlreadyHappened";
    public async Task<HttpResponseMessage> ExecuteActionFilterAsync(HttpActionContext actionContext,
        CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
    {
        using var execution = BeginExecution(actionContext);
        try
        {
            return await continuation();
        }
        finally
        {
            LogDetails(actionContext);
        }
    }

    protected virtual IDisposable? BeginExecution(HttpActionContext actionContext)
    {
        if (actionContext.ControllerContext.Controller is not DnnSxcControllerRoot controller)
            return null;
        var logger = controller.SysHlp.GetService<ILoggerFactory>().CreateLogger(MicrosoftLoggerEventSink.Category);
        var appId = actionContext.ActionArguments.TryGetValue("appId", out var value) && value is int id ? id : (int?)null;
        var operation = $"{actionContext.ControllerContext.ControllerDescriptor.ControllerName}.{actionContext.ActionDescriptor.ActionName}";
        return logger.BeginExecution(controller.Log, Activities, operation, appId: appId);
    }

    private static void LogDetails(HttpActionContext actionContext)
    {
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
    }

}
