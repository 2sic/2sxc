using ToSic.Eav.Context;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Sys.Configuration;

namespace ToSic.Sxc.Context.Sys;

partial class SxcCurrentContextService
{
    #region State as has been initialized.

    /// <summary>
    /// The simpler app context, can be App only or app with block.
    /// </summary>
    /// <remarks>
    /// This is set whenever an App Context or Block Context are set.
    /// </remarks>
    private IContextOfApp? AppContextFromAppOrBlock { get; set; }


    /// <summary>
    /// The block as is stored, or null; if no block is known.
    /// </summary>
    private IBlock? _blockOrNull;

    #endregion

    /// <inheritdoc/>
    public IBlock? BlockOrNull() => _blockOrNull;

    ///// <inheritdoc/>
    //public IContextOfApp AppContextRequired()
    //    => AppContextOrNull() ?? throw new($"To call {nameof(AppContextRequired)} first call {nameof(SetApp)}");

    /// <inheritdoc/>
    public IContextOfApp? AppContextOrNull() => AppContextFromAppOrBlock;


    public IContextOfApp ShareAppStateOptimized(int appId)
    {
        var l = Log.Fn<IContextOfApp>($"a#{appId}");
        // Get the current block context - but for clarity, get the block first, as we'll update it later
        var blockSpecs = _blockOrNull as BlockSpecs;

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
        
        _blockOrNull = blockSpecs with { Context = updatedBlockCtx };

        return l.Return(updatedBlockCtx);

        //// Helper function for re-use.
        //// New implementation 2025-03-31
        //// Previous version had edge cases where the wrong zone/app combinations were created
        //// Which would still load the App, but would result in 2 different AppIdentities being cached
        //// leading to unexpected results.
        //IAppIdentityPure SiteAppIdentity(int newAppId) => appReaderFactory.Value.AppIdentity(newAppId);
    }
}