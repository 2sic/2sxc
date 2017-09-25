using System.Web.Http.Filters;
using ToSic.Eav.Logging.Simple;
using ToSic.SexyContent.Razor.Helpers;
using ToSic.SexyContent.Environment.Dnn7;

namespace ToSic.SexyContent.WebApi
{
    public class WebApiLogDetails : ActionFilterAttribute
    {
        public override bool AllowMultiple => false;

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            base.OnActionExecuted(actionExecutedContext);
            try
            {
                // check if we have any logging details for this request
                if (!(actionExecutedContext.Request?.Properties.ContainsKey(Constants.EavLogKey) ??
                      false)) return;

                var log = actionExecutedContext.Request.Properties[Constants.EavLogKey] as Log;

                // check if we have additional context information (portal, module, etc.)
                actionExecutedContext.Request.Properties.TryGetValue(Constants.DnnContextKey,
                    out var dnnContext);

                Logging.LogToDnn("2sxc-Api", "Auto-Log", log, dnnContext as DnnHelper);
            }
            catch
            {
                Logging.TryToReportLoggingFailure("WebApiLogDetails");
            }
        }

    }
}
