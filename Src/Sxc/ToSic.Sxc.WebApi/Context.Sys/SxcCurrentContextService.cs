using ToSic.Eav.Apps.Sys.Permissions;
using ToSic.Sxc.Apps.Sys;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Blocks.Sys.Views;
using ToSic.Sxc.DataSources.Sys;
using ToSic.Sxc.Services;
using ToSic.Sxc.Sys.Configuration;
using ToSic.Sys.Context;
using ToSic.Sys.Users;

namespace ToSic.Sxc.Context.Sys;

/// <summary>
/// Main context resolver - provides context information about the site, app, block, permissions etc.
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class SxcCurrentContextService(
    ISite site, // IMPORTANT: Never import the IContextOfSiteService itself, since this class is registered to implement it.
    IUser user,
    Generator<IContextOfAppFactory> appCtxGenerator,
    LazySvc<IContextOfBlockFactory> blockCtxFactory,
    LazySvc<IFeaturesService> featuresService,
    LazySvc<IAppReaderFactory> appReaderFactory,
    LazySvc<BlockDataSourceFactory> bdsFactoryLazy,
    LazySvc<IDraftOverrideService> draftService)
    : ServiceBase("Sxc.CtxRes",
        connect: [appCtxGenerator, blockCtxFactory, featuresService, draftService, appReaderFactory, bdsFactoryLazy]),
        ISxcCurrentContextService
{
    /// <inheritdoc/>
    IContextOfUser IContextService.Context => Context;

    /// <inheritdoc/>
    public IContextOfSite Context
        => (IContextOfSite?)AppContextOrNull
           ?? (field ??= new ContextOfSite(site, user)
               { Permissions = draftService.Value.UserPermissions(new ContextOfSite(site, user).Permissions) });

    /// <inheritdoc/>
    public IBlock? BlockOrNull { get; private set; }

    /// <inheritdoc/>
    public IContextOfApp? AppContextOrNull { get; private set; }


    /// <inheritdoc/>
    public IContextOfApp ShareAppStateOptimized(int appId)
    {
        var l = Log.Fn<IContextOfApp>($"a#{appId}");
        // Get the current block context - but for clarity, get the block first, as we'll update it later
        var blockSpecs = BlockOrNull as BlockSpecs;

        // Option 1: No block context found, create a lightweight app context
        // This way security checks can only verify the App but not any module permissions
        if (blockSpecs?.Context is not { } blockCtx)
            return l.Return(ShareAppContext(appReaderFactory.Value.AppIdentity(appId)));

        // if there is a block context, make sure it's of the requested app (or no app was specified)
        // then return that
        // note: an edge case is that a block context exists, but no app was selected - then AppState is null

        // Option 2: The block context matches the requested App.
        // This is necessary when you come from a module which doesn't have an app yet (all apps menu).
        // Then you access an app, so the underlying AppReader is still null. 
        if (appId == blockCtx.AppReaderOrNull?.AppId)
            return l.Return(blockCtx);

        // Option 3: We're in an inner content, but should respect permissions of the outer block/module.
        // If the app in the request doesn't match the app in the context
        // Set the app in the context to the real one to be sure about security check
        // We still want to preserve the ModuleContext, so security checks can
        // still verify module permissions.
        // Only do this if the feature is enabled, as we are opening security when we do this
        if (!featuresService.Value.IsEnabled(SxcFeatures.PermissionPrioritizeModuleContext.NameId))
            // Get a simpler App-Context without the module context
            // so that module permissions will not allow editing of apps which are really in the current one.
            return l.Return(ShareAppContext(appReaderFactory.Value.AppIdentity(appId)));

        // Option 4: The block context exists, but the app is different.
        // Note that this is what was here before 2026-10-19 so we preserved it,
        // But it's not clear why this would be a real need.
        // If we figure this out, it should be documented here.
        //blockCtx.ResetApp(appReaderFactory.Value.AppIdentity(appId));

        var updatedBlockCtx = blockCtxFactory.Value.Update(blockCtx, appReaderFactory.Value.AppIdentity(appId));

        var result = ShareAndPatchBlockSpecs(blockSpecs with { Context = updatedBlockCtx });

        return l.Return(result.Context);
    }

    #region Block and View Setup / Swap

    public IBlock SwapBlockView(IView newView)
    {
        var blockSpecs = BlockOrNull as BlockSpecs
                         ?? throw new NullReferenceException("Block is not attached, cannot swap view.");

        // First set the view, since we need it to get the context data source
        var newSpecs = blockSpecs with
        {
            ViewOrNull = newView,
        };

        // This call must be after set-view, since it needs the view to get the underlying data-source
        var finalSpecs = newSpecs with
        {
            Data = bdsFactoryLazy.Value.GetContextDataSourceFromView(newSpecs, newSpecs.AppOrNull.TryGetAppLookUpEngineOrNull()),
        };
        ShareBlockSpecs(finalSpecs);
        return finalSpecs;
    }


    public void ShareBlockSpecs(IBlock block)
        => ShareAndPatchBlockSpecs(block);

    #endregion


    #region Private setters / State Updaters

    // Note: this is the only place where it's allowed to update _blockOrNull
    // to ensure that it reliably also updates the AppContext and permissions
    private IBlock ShareAndPatchBlockSpecs(IBlock block)
    {
        // Need block-specs and ctx as records for further processing
        var blockSpecs = block as BlockSpecs
                         ?? throw new NullReferenceException($"Block is not a {nameof(BlockSpecs)}, cannot update.");

        var origCtx = blockSpecs.Context as ContextOfBlock
                      ?? throw new NullReferenceException("Context is not Block or App, cannot update.");
        
        var updatedPerms = draftService.Value.UserPermissions(origCtx.Permissions);
        
        BlockOrNull = blockSpecs with { Context = origCtx with { Permissions = updatedPerms } };
        AppContextOrNull = BlockOrNull.Context;
        return BlockOrNull;
    }

    /// <summary>
    /// Basic inject app.
    /// </summary>
    /// <remarks>
    /// It's important to know that the AppContextGenerator will already
    /// check for the additional cookie permissions, so it doesn't need to happen here.
    /// </remarks>
    private IContextOfApp ShareAppContext(IAppIdentity appIdentity)
        => AppContextOrNull = appCtxGenerator.New().Create(appIdentity);

    #endregion
    
}