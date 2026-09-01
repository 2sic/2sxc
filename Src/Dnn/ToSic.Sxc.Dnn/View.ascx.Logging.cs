using System.Web.Http;
using ToSic.Sxc.Dnn.Run;

namespace ToSic.Sxc.Dnn;

partial class View
{
    /// <summary>
    /// Optional detailed logging to include in the HTML source
    /// </summary>
    /// <returns></returns>
    private string GetOptionalDetailedLogToAttach()
    {
        try
        {
            // if in debug mode and is super-user (or it's been enabled for all), then add to page debug
            if (Request.QueryString["debug"] == "true")
                if (UserInfo.IsSuperUser
                    || DnnLogging.EnableLogging(GlobalConfiguration.Configuration.Properties))
                    return Log.Dump(" - ", "<!-- 2sxc insights for " + ModuleId + "\n", "-->");
        }
        catch { /* ignore */ }

        return "";
    }
}