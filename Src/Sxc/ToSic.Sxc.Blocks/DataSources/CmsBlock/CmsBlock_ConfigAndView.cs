using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Apps.Views.Sys;
using ToSic.Eav.DataSource.Sys.Errors;
using ToSic.Sxc.Blocks.Sys;

namespace ToSic.Sxc.DataSources;

partial class CmsBlock
{
    private record ConfigAndView(BlockConfiguration BlockConfiguration, IView View);

    private ResultOrError<ConfigAndView> ConfigAndViewOrErrors => field
        ??= LoadConfigAndViewOrErrors();

    /// <summary>
    /// Load configuration and view information.
    /// </summary>
    /// <returns></returns>
    private ResultOrError<ConfigAndView> LoadConfigAndViewOrErrors()
    {
        var config = LoadBlockConfiguration();
        if (!config.IsOk)
            return new(false, default, config.ErrorsSafe());

        var view = config.Result.View;
        return view == null
            // problem, no view
            ? new(false, default, Error.Create(title: "CmsBlock View Missing",
                message: "Cannot find View configuration of current CmsBlock"))
            // all ok 
            : new(true, new(config.Result, view));
    }

    /// <summary>
    /// Load block configuration - from context or from module definition.
    /// </summary>
    /// <returns></returns>
    private ResultOrError<BlockConfiguration> LoadBlockConfiguration()
    {
        var l = Log.Fn<ResultOrError<BlockConfiguration>>();

        // Case 1: Check if the context injected a block specs
        if (blockCtx.ValueOptional is { } block)
            return l.Return(new(true, block.Configuration),"will use from ContentGroup from context");

        // Case 2: If we don't have a context, then look it up based on the Module Information
        l.A("need content-group, will construct as cannot use context");
        if (ModuleId is not { } mid)
            return l.Return(new(false, null, Error.Create(
                    title: $"{nameof(CmsBlock)} cannot find Block Configuration",
                    message: $"Neither InstanceContext nor {nameof(ModuleId)} found")),
                message: "Error, no module-id");

        // Find block identifier
        // todo: API says BlockIdentifier could be null,
        // but it's unclear if this could really ever be null.
        // We must wait for Oqtane module factory to complete, then we can clean up the API and remove the "!"
        var blockId = moduleFactory.Value.Get(mid).BlockIdentifier!;

        // Generate config based on the block identifier
        var blockConfig = workBlocks.New(this.PureIdentity())
            .GetOrGeneratePreviewConfig(blockId);
        return l.Return(new(true, blockConfig), "ok");
    }
}
