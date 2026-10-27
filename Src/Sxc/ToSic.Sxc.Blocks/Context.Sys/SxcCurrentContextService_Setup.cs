using ToSic.Eav.Apps;
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
    private IContextOfApp ShareAppContext(IAppIdentity appIdentity)
        => AppContextOrNull = appCtxGenerator.New().Create(appIdentity);


    #region Block Setup

    public void ShareBlockSpecs(IBlock block) => ShareAndPatchBlockSpecs(block);

    // Note: this is the only place where it's allowed to update _blockOrNull
    // to ensure that it reliably also updates the AppContext and permissions
    public IBlock ShareAndPatchBlockSpecs(IBlock block)
    {
        var blockSpecs = block as BlockSpecs
                         ?? throw new NullReferenceException($"Block is not a {nameof(BlockSpecs)}, cannot update context.");
        var ctxWithUpdatedDrafting = CtxWithUpdatedDrafting(blockSpecs.Context);

        AppContextOrNull = ctxWithUpdatedDrafting;
        return BlockOrNull = blockSpecs with { Context = ctxWithUpdatedDrafting };
    }

    #endregion

    #region View Setup / Swap

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

    #endregion

}
