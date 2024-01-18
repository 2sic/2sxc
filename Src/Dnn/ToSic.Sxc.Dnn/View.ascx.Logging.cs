using System.Web.Http;
using ToSic.Sxc.Dnn.Run;

namespace ToSic.Sxc.Dnn;

partial class View
{
    /// <summary>
    /// Html Comment containing the log for the current module
    /// </summary>
    /// <returns></returns>
    private string HtmlLog()
        => Log.Dump(" - ", "<!-- 2sxc insights for " + ModuleId + "\n", "-->");

    /// <summary>
    /// optional detailed logging
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
                    return HtmlLog();
        }
        catch { /* ignore */ }

        return "";
    }
}