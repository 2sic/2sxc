using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Internal;
using ToSic.Lib.Services;
using ToSic.Sxc.Web;
using ToSic.Sxc.Web.Internal.DotNet;

namespace ToSic.Sxc.Context.Internal;

/// <summary>
/// This helps API calls to get the app which is currently needed
/// It does not perform security checks ATM and maybe never will
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class AppIdResolver: ServiceBase
{
    #region Constructor & DI

    public AppIdResolver(IHttp http, AppFinder appFinder): base("Api.FindAp")
    {
        ConnectServices(
            _http = http,
            _appFinder = appFinder
        );
    }
    private readonly IHttp _http;
    private readonly AppFinder _appFinder;

    #endregion

    /// <summary>
    /// New implementation to replace previous
    /// </summary>
    /// <returns></returns>
    internal int GetAppIdFromPath(int zoneId, string appPath, bool required)
    {
        var wrapLog = Log.Fn<int>($"{zoneId}, {appPath}, {required}");
        // get app from AppName
        var aid = _appFinder/* _zoneRuntime.Init(zoneId, Log)*/.FindAppId(zoneId, appPath, true);
        if (aid <= Eav.Constants.AppIdEmpty && required)
            throw new($"App required but can't find App based on the name '{appPath}'");

        return wrapLog.Return(aid, $"found app:{aid}");
    }


    /// <summary>
    /// This will detect the app based on appid/zoneid params in the URL
    /// It's a temporary solution, because normally we want the control flow to be more obvious
    /// </summary>
    /// <returns></returns>
    internal IAppIdentity GetAppIdFromRoute()
    {
        var allUrlKeyValues = _http.QueryStringKeyValuePairs();
        var ok1 = int.TryParse(allUrlKeyValues.FirstOrDefault(
                x => x.Key.Equals(ContextConstants.ZoneIdKey, StringComparison.InvariantCultureIgnoreCase)).Value, 
            out var zoneId);
        var ok2 = int.TryParse(allUrlKeyValues.FirstOrDefault(
                x => x.Key.Equals(ContextConstants.AppIdKey, StringComparison.InvariantCultureIgnoreCase)).Value, 
            out var appId);

        if (!ok1 || !ok2) return null;

        Log.A($"Params in URL detected - will use appId:{appId}, zoneId:{zoneId}");
        return new AppIdentity(zoneId, appId);
    }

}