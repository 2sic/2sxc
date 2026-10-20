using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Context;
using ToSic.Sxc.Apps.Sys;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Blocks.Sys.Views;

namespace ToSic.Sxc.Context.Sys;

partial class SxcCurrentContextService
{
    /// <summary>
    /// Basic inject app.
    /// </summary>
    private IContextOfApp ShareAppState(IAppIdentity appIdentity)
        => ShareAppState(appCtxGenerator.New().Create(appIdentity));

    [return: NotNullIfNotNull(nameof(appContext))]
    private IContextOfApp? ShareAppState(IContextOfApp? appContext)
    {
        if (appContext == null)
            return null;
        return AppContextFromAppOrBlock = appContext;
    }


    #region Inject App using Name

    public IContextOfApp? FindAppFromPathOrNull(string? nameOrPath)
    {
        if (string.IsNullOrWhiteSpace(nameOrPath))
            return null;
        var zoneId = SiteContext.Site.ZoneId;
        var appId = appIdResolverLazy.Value.GetAppIdFromAppPathOrName(zoneId, nameOrPath!, throwIfNotFound: false);
        return appId <= KnownAppsConstants.AppIdEmpty
            ? null
            : appCtxGenerator.New().Create(new AppIdentity(zoneId, appId));
    }

    public IContextOfApp? SetAppFromPathOrNull(string? nameOrPath)
        => ShareAppState(FindAppFromPathOrNull(nameOrPath));

    public IContextOfApp SetAppFromPathOrGetExistingBlock(string nameOrPath)
        => SetAppFromPathOrNull(nameOrPath)
           ?? _blockOrNull?.Context ?? throw new($"Tried to get app by name '{nameOrPath}', but no existing block context is available.");

    public IContextOfApp SetAppFromNameOrUrlParamsOrGetExistingBlock(string? nameOrPath)
    {
        var ctx = SetAppFromPathOrNull(nameOrPath);
        if (ctx != null)
            return ctx;

        var identity = appIdResolverLazy.Value.GetAppIdFromUrlParams();
        if (identity != null)
            return ShareAppState(identity);

        return this._blockOrNull?.Context
               ?? throw new($"Tried to auto detect app by name '{nameOrPath}', url params or block context, all failed.");
    }


    #endregion


    #region Block Setup

    public void ShareBlockSpecs(IBlock block)
    {
        _blockOrNull = block;
        AppContextFromAppOrBlock = _blockOrNull?.Context;
    }

    #endregion

    #region View Setup / Swap

    public IBlock SwapBlockView(IView newView)
    {
        var blockSpecs = _blockOrNull as BlockSpecs
                         ?? throw new NullReferenceException("Block is not attached, cannot swap view.");

        var newSpecs = blockSpecs with
        {
            ViewOrNull = newView,
        };

        // This call must be after set-view, since it needs the view to get the underlying data-source
        newSpecs = newSpecs with
        {
            Data = bdsFactoryLazy.Value.GetContextDataSourceFromView(newSpecs, newSpecs.AppOrNull.TryGetAppLookUpEngineOrNull())
        };
        return _blockOrNull = newSpecs;
    }

    #endregion

}