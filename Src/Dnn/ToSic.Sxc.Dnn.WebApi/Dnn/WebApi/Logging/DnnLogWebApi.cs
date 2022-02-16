using System.Web.Http.Filters;
using ToSic.Eav.Logging;
using ToSic.Sxc.Dnn.Context;
using ToSic.Sxc.Dnn.Run;

namespace ToSic.Sxc.Dnn.WebApi.Logging
{
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
                // check if we have any logging details for this request
                if (!reqProps.ContainsKey(DnnConstants.EavLogKey)) return;

                if (reqProps.ContainsKey(AlreadyLogged)) return;

                var log = reqProps[DnnConstants.EavLogKey] as ILog;

                // check if we have additional context information (portal, module, etc.)
                reqProps.TryGetValue(DnnConstants.DnnContextKey, out var dnnContext);

                DnnLogging.LogToDnn("2sxc-Api", 
                    actionContext.Request.RequestUri.PathAndQuery, 
                    log, 
                    dnnContext as DnnContext);

                // set property, to prevent double-logging
                actionContext.Request.Properties.Add(AlreadyLogged, true);
            }
            catch
            {
                DnnLogging.TryToReportLoggingFailure("WebApiLogDetails");
            }
        }

    }
}
