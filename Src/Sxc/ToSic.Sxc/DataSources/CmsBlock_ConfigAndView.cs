using ToSic.Eav.Apps.Parts;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSources;
using ToSic.Lib.Helpers;
using ToSic.Lib.Logging;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Blocks;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment


namespace ToSic.Sxc.DataSources
{
    public sealed partial class CmsBlock
    {
        private ResultOrError<(BlockConfiguration BlockConfiguration, IView View)> ConfigAndViewOrErrors => _everything.Get(() =>
        {
            var config = LoadBlockConfiguration();
            if (config.IsError)
                return new ResultOrError<(BlockConfiguration BlockConfiguration, IView view)>(false, default,
                    config.Errors);

            var view = OverrideView ?? config.Result.View;
            if (view == null)
                return new ResultOrError<(BlockConfiguration BlockConfiguration, IView view)>(false, default,
                    Error.Create(title: "CmsBlock View Missing",
                        message: "Cannot find View configuration of current CmsBlock"));
            // all ok 
            return new ResultOrError<(BlockConfiguration BlockConfiguration, IView view)>(true, (config.Result, view));
        });

        private readonly GetOnce<ResultOrError<(BlockConfiguration blockConfiguration, IView view)>> _everything =
            new GetOnce<ResultOrError<(BlockConfiguration blockConfiguration, IView view)>>();

        private ResultOrError<BlockConfiguration> LoadBlockConfiguration() => Log.Func(l =>
        {
            if (UseSxcInstanceContentGroup)
                return (new ResultOrError<BlockConfiguration>(true, Block.Configuration), "need content-group, will use from Sxc Instance ContentGroup");

            // If we don't have a context, then look it up based on the InstanceId
            l.A("need content-group, will construct as cannot use context");
            Configuration.Parse();
            if (!ModuleId.HasValue)
                return (new ResultOrError<BlockConfiguration>(false, null,
                    Error.Create(title: $"{nameof(CmsBlock)} cannot find Block Configuration",
                        message: $"Neither InstanceContext nor {nameof(ModuleId)} found")), "Error, no module-id");

            var cms = _services.LazyCmsRuntime.IsValueCreated
                ? _services.LazyCmsRuntime.Value
                : _services.LazyCmsRuntime.Value.InitQ(this);
            var container = _services.ModuleLazy.Value.Init(ModuleId.Value);
            var blockId = container.BlockIdentifier;
            return (new ResultOrError<BlockConfiguration>(true, cms.Blocks.GetOrGeneratePreviewConfig(blockId)), "ok");
        });


        /// <summary>
        /// This allows external settings to override the view given by the configuration. This is used to temporarily use an alternate view.
        /// For example, when previewing a different template. 
        /// </summary>
        public IView OverrideView { get; set; }


    }
}
