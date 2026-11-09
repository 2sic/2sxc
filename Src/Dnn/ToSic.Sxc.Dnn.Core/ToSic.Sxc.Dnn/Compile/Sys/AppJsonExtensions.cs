using ToSic.Eav.Apps.Sys.AppJson;

namespace ToSic.Sxc.Dnn.Compile.Sys;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public static class AppJsonExtensions
{

    /// <summary>
    /// Check that the app is configured in app.json to always use Roslyn compiler
    /// </summary>
    /// <param name="appJsonService"></param>
    /// <param name="appId"></param>
    /// <returns></returns>
    public static bool DnnCompilerAlwaysUseRoslyn(this IAppJsonConfigurationService appJsonService, int appId) 
        => appJsonService.GetAppJson(appId)?.DotNet?.Compiler?.Equals("roslyn", StringComparison.OrdinalIgnoreCase) == true;
}