using System.Web.Http.Filters;
using ToSic.Eav.Generics;
using ToSic.Lib.Logging;
using ToSic.Sxc.Dnn.Run;

namespace ToSic.Sxc.Dnn.WebApi.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class DnnLogWebApi : ActionFilterAttribute
{
    public override bool AllowMultiple => false;

    const string AlreadyLogged = "LogDetailsAlreadyHappened";

    public override void OnActionExecuted(HttpActionExecutedContext actionContext)
    {
        base.OnActionExecuted(actionContext);

        try
        {
            var reqProps = actionContext.Request.Properties;

            // check if already logged, and set property to prevent double-logging
            if (reqProps.ContainsKey(AlreadyLogged)) return;
            reqProps.Add(AlreadyLogged, true);

            // check if we have any logging details for this request
            if (!reqProps.TryGetTyped(DnnConstants.EavLogKey, out LogStoreEntry logStoreEntry)) return;

            // check if we have additional context information (portal, module, etc.)
            reqProps.TryGetValue(DnnConstants.DnnContextKey, out var dnnContext);

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