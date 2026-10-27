using ToSic.Eav.Apps.Sys;
using ToSic.Sxc.Web.Sys.Http;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Context.Sys;

/// <summary>
/// This helps API calls to get the app which is currently needed
/// It does not perform security checks ATM and maybe never will
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class ContextBuilderService(ISite site, LazySvc<IHttp> http, LazySvc<AppFinder> appFinder, Generator<IContextOfAppFactory> appCtxGenerator)
    : ServiceBase("Api.FindAp", connect: [http, appFinder, appCtxGenerator]), IContextBuilderService
{
    #region Inject App using Name

    public IContextOfApp? TryFindAppFromPath(string? nameOrPath)
    {
        if (string.IsNullOrWhiteSpace(nameOrPath))
            return null;
        var zoneId = site.ZoneId;
        var appId = GetAppIdFromAppPathOrName(zoneId, nameOrPath!, throwIfNotFound: false);
        return appId <= KnownAppsConstants.AppIdEmpty
            ? null
            : appCtxGenerator.New().Create(new AppIdentity(zoneId, appId));
    }


    public IContextOfApp? TryFindAppFromUrlIds()
    {
        // Priority 2: Try to find app by URL params (appId=x;zoneId=y) - not used much AFAIK
        var identity = GetAppIdFromUrlParams();
        return identity != null
            ? appCtxGenerator.New().Create(identity)
            : null;
    }

    #endregion

    /// <summary>
    /// Try to find the AppId based on the apps path or name.
    /// Optionally throw an exception if not found (as of 2026-10 this is not used).
    /// </summary>
    /// <returns></returns>
    private int GetAppIdFromAppPathOrName(int zoneId, string appPathOrName, bool throwIfNotFound)
    {
        var l = Log.Fn<int>($"{zoneId}, {appPathOrName}, {throwIfNotFound}");
        // get app from AppName
        var aid = appFinder.Value.FindAppId(zoneId, appPathOrName, alsoCheckFolderName: true);
        if (aid <= KnownAppsConstants.AppIdEmpty && throwIfNotFound)
            throw new($"App required but can't find App based on the name '{appPathOrName}'");

        return l.Return(aid, $"found app:{aid}");
    }


    /// <summary>
    /// This will detect the app based on appid/zoneid params in the URL
    /// It's a temporary solution, because normally we want the control flow to be more obvious
    /// </summary>
    /// <returns></returns>
    private IAppIdentity? GetAppIdFromUrlParams()
    {
        var l = Log.Fn<IAppIdentity?>();
        var allUrlKeyValues = http.Value.QueryStringKeyValuePairs();
        var zoneIdOk = int.TryParse(
            allUrlKeyValues.FirstOrDefault(x => x.Key.EqualsInsensitive(ContextConstants.ZoneIdKey)).Value,
            out var zoneId
        );
        var appIdOk = int.TryParse(
            allUrlKeyValues.FirstOrDefault(x => x.Key.EqualsInsensitive(ContextConstants.AppIdKey)).Value,
            out var appId
        );

        if (!zoneIdOk || !appIdOk)
            return l.ReturnNull("either zoneId or appId not valid");

        return l.Return(new AppIdentity(zoneId, appId), $"Params in URL detected - will use appId:{appId}, zoneId:{zoneId}");
    }

}