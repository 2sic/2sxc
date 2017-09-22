using System;
using System.Web.Http.Filters;
using ToSic.Eav.Logging.Simple;

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
                var props = actionExecutedContext.ActionContext?.ControllerContext?.Configuration?.Properties;
                if (props != null && props.TryGetValue(Constants.AdvancedLoggingEnabledKey, out object enabled))
                {
                    if (!(enabled is bool)) return;
                    if (!(bool) enabled) return;

                    if (props.TryGetValue(Constants.AdvancedLoggingTillKey, out object till))
                    {
                        if (!(till is DateTime)) return;
                        var dtmTill = (DateTime) till;
                        if (dtmTill.CompareTo(DateTime.Now) <= 0) return;

                        if (!(actionExecutedContext.Request?.Properties.ContainsKey(Constants.EavLogKey) ??
                              false)) return;

                        var log = actionExecutedContext.Request.Properties[Constants.EavLogKey] as Log;
                        Environment.Dnn7.Logging.LogToDnn("Api", "Auto-Log", log);
                    }
                }
            }
            catch { }
        }
    }
}
