using ToSic.Eav.Apps.Internal;

namespace ToSic.Sxc.Dnn.Compile.Internal;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public static class AppJsonExtensions
{

    /// <summary>
    /// Check that the app is configured in app.json to always use Roslyn compiler
    /// </summary>
    /// <param name="appJsonService"></param>
    /// <param name="appId"></param>
    /// <returns></returns>
    public static bool DnnCompilerAlwaysUseRoslyn(this IAppJsonService appJsonService, int appId) 
        => appJsonService.GetAppJson(appId)?.DotNet?.Compiler?.Equals("roslyn", StringComparison.OrdinalIgnoreCase) == true;
}