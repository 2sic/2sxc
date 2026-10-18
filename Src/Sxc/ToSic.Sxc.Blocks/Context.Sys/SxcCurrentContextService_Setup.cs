using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Context;
using ToSic.Sxc.Apps.Sys;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Blocks.Sys.Views;

namespace ToSic.Sxc.Context.Sys;

partial class SxcCurrentContextService
{
    public IContextOfApp SetApp(IAppIdentity appIdentity)
    {
        var appCtx = appCtxGenerator.New();
        appCtx.ResetApp(appIdentity);
        AppContextFromAppOrBlock = appCtx;
        return appCtx;
    }

    public IContextOfApp? SetAppOrNull(string? nameOrPath)
    {
        if (string.IsNullOrWhiteSpace(nameOrPath))
            return null;
        var zoneId = SiteContext.Site.ZoneId;
        var appId = appIdResolverLazy.Value.GetAppIdFromPath(zoneId, nameOrPath!, false);
        return appId <= KnownAppsConstants.AppIdEmpty
            ? null
            : SetApp(new AppIdentity(zoneId, appId));
    }

    #region Blocks

    public void AttachBlock(IBlock block)
    {
        _block = block;
        AppContextFromAppOrBlock = _block?.Context;
    }

    #endregion

    #region View

    public IBlock SwapBlockView(IView newView)
    {
        var blockSpecs = _block as BlockSpecs
                         ?? throw new NullReferenceException("Block is not attached, cannot swap view.");

        var newSpecs = blockSpecs with
        {
            ViewOrNull = newView,
        };

        // This call must be after set-view, since it needs the view to get the data-source
        newSpecs = newSpecs with
        {
            Data = bdsFactoryLazy.Value.GetContextDataSourceFromView(newSpecs, newSpecs.AppOrNull.TryGetAppLookUpEngineOrNull())
        };
        _block = newSpecs;
        return _block;
    }

    #endregion

    //public IContextOfApp GetExistingAppOrSet(int appId)
    //{
    //    var l = Log.Fn<IContextOfApp>($"a#{appId}");
    //    // get the current block context
    //    var moduleCtx = this.BlockContextOrNull();

    //    // If there is no block context, then just use a "blank" app context
    //    // This way security checks can only verify the App but not any module permissions
    //    if (moduleCtx == null)
    //        return l.Return(SetApp(SiteAppIdentity()));

    //    // if there is a block context, make sure it's of the requested app (or no app was specified)
    //    // then return that
    //    // note: an edge case is that a block context exists, but no app was selected - then AppState is null


    //    // If the app in the request matches the app in the context, everything is fine
    //    // This is necessary when you come from a module which doesn't have an app yet (all apps menu).
    //    // Then you access an app, so the underlying AppReader is still null. 
    //    if (appId == moduleCtx.AppReaderOrNull?.AppId)
    //        return l.Return(moduleCtx);

    //    // If the app in the request doesn't match the app in the context
    //    // Set the app in the context to the real one to be sure about security check
    //    // We still want to preserve the ModuleContext, so security checks can
    //    // still verify module permissions.
    //    // Only do this if the feature is enabled, as we are opening security when we do this
    //    if (!featuresService.Value.IsEnabled(SxcFeatures.PermissionPrioritizeModuleContext.NameId))
    //        // Get a simpler App-Context without the module context
    //        // so that module permissions will not allow editing of apps which are really in the current one.
    //        return l.Return(SetApp(SiteAppIdentity()));

    //    moduleCtx.ResetApp(SiteAppIdentity());
    //    return l.Return(moduleCtx);

    //    // Old implementation
    //    //AppIdentity SiteAppIdentity() => new(Site().Site.ZoneId, appId);

    //    // New implementation 2025-03-31
    //    // Previous version had edge cases where the wrong zone/app combinations were created
    //    // Which would still load the App, but would result in 2 different AppIdentities being cached
    //    // leading to unexpected results.
    //    IAppIdentityPure SiteAppIdentity() => appReaderFactory.Value.AppIdentity(appId);
    //}


    //public IContextOfApp SetAppOrGetBlock(string nameOrPath)
    //    => SetAppOrNull(nameOrPath) ?? this.BlockContextRequired();


    //public IContextOfApp AppNameRouteBlock(string? nameOrPath)
    //{
    //    var ctx = SetAppOrNull(nameOrPath);
    //    if (ctx != null)
    //        return ctx;

    //    var identity = appIdResolverLazy.Value.GetAppIdFromRoute();
    //    if (identity != null)
    //        return SetApp(identity);

    //    ctx = this.BlockContextOrNull();
    //    return ctx ?? throw new($"Tried to auto detect app by name '{nameOrPath}', url params or block context, all failed.");
    //}

}