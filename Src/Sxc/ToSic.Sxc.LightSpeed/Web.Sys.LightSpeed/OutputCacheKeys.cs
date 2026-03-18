namespace ToSic.Sxc.Web.Sys.LightSpeed;
public class OutputCacheKeys
{
    internal const string GlobalCacheKeyModuleRoot = "Sxc-LightSpeed.Module.";
    internal const string GlobalCacheKeyPartialRoot = "Sxc-LightSpeed.Partial.";
    internal const string GlobalCacheKeyAppDependencyRoot = "Sxc-LightSpeed.App.";
    internal const string GlobalCacheKeyExternalDependencyRoot = "Sxc-LightSpeed.Ext.";

    /// <summary>
    /// Determine the cache key for module data.
    /// </summary>
    /// <param name="pageId"></param>
    /// <param name="moduleId"></param>
    /// <param name="userId">optional, set if vary-by-user</param>
    /// <param name="viewKey">View key is important in case the user is doing previews and temporarily changing the view</param>
    /// <param name="suffix">url parameters which are cache relevant</param>
    /// <param name="currentCulture"></param>
    /// <returns></returns>
    internal static string ModuleKey(int pageId, int moduleId, int? userId, string? viewKey, string? suffix, string? currentCulture)
    {
        var id = $"{GlobalCacheKeyModuleRoot}p:{pageId}-m:{moduleId}";
        if (userId.HasValue)
            id += $"-u:{userId.Value}";
        if (viewKey != null)
            id += $"-v:{viewKey}";
        if (suffix != null)
            id += $"-s:{suffix}";
        if (currentCulture != null)
            id += $"-c:{currentCulture}";
        return id;
    }
    //public static string PartialSettingsKey(int appId, string path /*, int moduleId, int? userId, string? viewKey, string? suffix,  string? currentCulture */)
    //    => PartialSettingsKey(appId.ToString(), path);

    public static string PartialSettingsKey(string appKey, string path /*, int moduleId, int? userId, string? viewKey, string? suffix,  string? currentCulture */)
    {
        var id = $"{GlobalCacheKeyPartialRoot}Settings.a:{appKey}-p:{path}";
        //if (userId.HasValue)
        //    id += $"-u:{userId.Value}";
        //if (viewKey != null)
        //    id += $"-v:{viewKey}";
        //if (suffix != null)
        //    id += $"-s:{suffix}";
        //if (currentCulture != null)
        //    id += $"-c:{currentCulture}";
        return id;
    }

    //public static string PartialKey(int appId, string path /*, int moduleId, int? userId, string? viewKey, string? suffix,  string? currentCulture */)
    //    => PartialKey(appId.ToString(), path);

    public static string PartialKey(string appKey, string path /*, int moduleId, int? userId, string? viewKey, string? suffix,  string? currentCulture */)
    {
        var id = $"{GlobalCacheKeyPartialRoot}a:{appKey}-p:{path}";
        //if (userId.HasValue)
        //    id += $"-u:{userId.Value}";
        //if (viewKey != null)
        //    id += $"-v:{viewKey}";
        //if (suffix != null)
        //    id += $"-s:{suffix}";
        //if (currentCulture != null)
        //    id += $"-c:{currentCulture}";
        return id;
    }

    internal static string ExternalDependencyKey(int appId, string dependencyName)
        => $"{GlobalCacheKeyExternalDependencyRoot}a:{appId}-k:{dependencyName.Trim().ToLowerInvariant()}";

    internal static string AppDependencyKey(int appId)
        => $"{GlobalCacheKeyAppDependencyRoot}a:{appId}";
}
