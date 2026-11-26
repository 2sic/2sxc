using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Apps.Views.Sys;
using ToSic.Eav.DataSource.Sys.Errors;
using ToSic.Sxc.Blocks.Sys;

namespace ToSic.Sxc.DataSources;

partial class CmsBlock
{
    private ResultOrError<(BlockConfiguration BlockConfiguration, IView View)> ConfigAndViewOrErrors
        => field ??= GetConfigViewOrErrors();

    private ResultOrError<(BlockConfiguration BlockConfiguration, IView View)> GetConfigViewOrErrors()
    {
        var config = LoadBlockConfiguration();
        if (!config.IsOk)
            return new ResultOrError<(BlockConfiguration BlockConfiguration, IView view)>(false, default,
                config.ErrorsSafe());

        var view = config.Result.View;
        if (view == null)
            return new ResultOrError<(BlockConfiguration BlockConfiguration, IView view)>(false, default,
                Error.Create(title: "CmsBlock View Missing",
                    message: "Cannot find View configuration of current CmsBlock"));
        // all ok 
        return new ResultOrError<(BlockConfiguration BlockConfiguration, IView view)>(true, (config.Result, view));
    }

    private ResultOrError<BlockConfiguration> LoadBlockConfiguration()
    {
        var l = Log.Fn<ResultOrError<BlockConfiguration>>();
        if (UseSxcInstanceContentGroup)
            return l.Return(new(true, Block.Configuration), "need content-group, will use from Sxc Instance ContentGroup");

        // If we don't have a context, then look it up based on the InstanceId
        l.A("need content-group, will construct as cannot use context");
        Configuration.Parse();
        if (!ModuleId.HasValue)
            return l.Return(new(false, null,
                Error.Create(title: $"{nameof(CmsBlock)} cannot find Block Configuration",
                    message: $"Neither InstanceContext nor {nameof(ModuleId)} found")), "Error, no module-id");

        var container = _services.ModuleLazy.Value.Get(ModuleId.Value);
        // container.Init(ModuleId.Value);
        var blockId = container.BlockIdentifier;
        var ctx = _services.AppCtxSvc.Value.ContextNew(this.PureIdentity());
        var blockConfig = _services.AppBlocks.New(ctx).GetOrGeneratePreviewConfig(blockId);
        return l.Return(new(true, blockConfig), "ok");
    }
}
