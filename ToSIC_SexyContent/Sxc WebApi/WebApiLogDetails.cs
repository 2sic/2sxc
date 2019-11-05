using System.Web.Http.Filters;
using ToSic.Eav.Logging;
using ToSic.SexyContent.Razor.Helpers;
using ToSic.SexyContent.Environment.Dnn7;

namespace ToSic.SexyContent.WebApi
{
    public class WebApiLogDetails : ActionFilterAttribute
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
                if (!reqProps.ContainsKey(Constants.EavLogKey)) return;

                if (reqProps.ContainsKey(AlreadyLogged)) return;

                var log = reqProps[Constants.EavLogKey] as ILog;

                // check if we have additional context information (portal, module, etc.)
                reqProps.TryGetValue(Constants.DnnContextKey, out var dnnContext);

                Logging.LogToDnn("2sxc-Api", 
                    actionContext.Request.RequestUri.PathAndQuery, 
                    log, 
                    dnnContext as DnnHelper);

                // set property, to prevent double-logging
                actionContext.Request.Properties.Add(AlreadyLogged, true);
            }
            catch
            {
                Logging.TryToReportLoggingFailure("WebApiLogDetails");
            }
        }

    }
}
