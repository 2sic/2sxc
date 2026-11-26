using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Apps.Views.Sys;
using ToSic.Eav.DataSource.Sys.Errors;
using ToSic.Sxc.Blocks.Sys;

namespace ToSic.Sxc.DataSources;

partial class CmsBlock
{
    internal bool UseSxcInstanceContentGroup = false;

    private record ConfigAndView(BlockConfiguration BlockConfiguration, IView View);

    private ResultOrError<ConfigAndView> ConfigAndViewOrErrors
        => field ??= GetConfigViewOrErrors();

    private ResultOrError<ConfigAndView> GetConfigViewOrErrors()
    {
        var config = LoadBlockConfiguration(UseSxcInstanceContentGroup);
        if (!config.IsOk)
            return new(false, default, config.ErrorsSafe());

        var view = config.Result.View;
        if (view == null)
            return new(false, default, Error.Create(title: "CmsBlock View Missing",
                message: "Cannot find View configuration of current CmsBlock"));
        // all ok 
        return new(true, new(config.Result, view));
    }

    private ResultOrError<BlockConfiguration> LoadBlockConfiguration(bool useSxcInstanceContentGroup)
    {
        var l = Log.Fn<ResultOrError<BlockConfiguration>>();
        if (useSxcInstanceContentGroup)
        {
            var block = new CmsBlockInstanceFinder().GetFromConfiguration(Configuration);
            return l.Return(new(true, block.Configuration),
                "need content-group, will use from Sxc Instance ContentGroup");
        }

        // If we don't have a context, then look it up based on the InstanceId
        l.A("need content-group, will construct as cannot use context");
        Configuration.Parse();
        if (!ModuleId.HasValue)
            return l.Return(new(false, null,
                Error.Create(title: $"{nameof(CmsBlock)} cannot find Block Configuration",
                    message: $"Neither InstanceContext nor {nameof(ModuleId)} found")), "Error, no module-id");

        var container = moduleLazy.Value.Get(ModuleId.Value);
        var blockId = container.BlockIdentifier!; // todo: unclear if this could really ever be null, must wait for Oqtane module factory to complete
        var blockConfig = appBlocks.New(this.PureIdentity()).GetOrGeneratePreviewConfig(blockId);
        return l.Return(new(true, blockConfig), "ok");
    }
}
