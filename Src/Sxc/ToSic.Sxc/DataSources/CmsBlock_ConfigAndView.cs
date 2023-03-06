using ToSic.Eav.Apps.Parts;
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
        //private BlockConfiguration BlockConfiguration => _blockConfiguration ?? (_blockConfiguration = LoadBlockConfiguration());
        //private BlockConfiguration _blockConfiguration;


        private ResultOrError<(BlockConfiguration BlockConfiguration, IView View)> Everything => _everything.Get(() =>
        {
            var config = LoadBlockConfiguration();
            if (config.IsError)
                return new ResultOrError<(BlockConfiguration BlockConfiguration, IView view)>(false, default,
                    () => config.Errors);

            var view = OverrideView ?? config.Result.View;
            if (view == null)
                return new ResultOrError<(BlockConfiguration BlockConfiguration, IView view)>(false, default, () =>
                    ErrorHandler.CreateErrorList(title: "CmsBlock View Missing",
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
            if (!ModuleId.HasValue)
                return (new ResultOrError<BlockConfiguration>(false, null,
                    () => CreateError($"{nameof(CmsBlock)} cannot find Block Configuration",
                        $"Neither InstanceContext nor {nameof(ModuleId)} found")), "Error, no module-id");

            var userMayEdit = HasInstanceContext && Block.Context.UserMayEdit;

            var cms = _services.LazyCmsRuntime.IsValueCreated
                ? _services.LazyCmsRuntime.Value
                : _services.LazyCmsRuntime.Value.InitQ(this, userMayEdit);
            var container = _services.ModuleLazy.Value.Init(ModuleId.Value);
            var blockId = container.BlockIdentifier;
            return (new ResultOrError<BlockConfiguration>(true, cms.Blocks.GetOrGeneratePreviewConfig(blockId)), "ok");
        });


        //private IView View => _view ?? (_view = OverrideView ?? BlockConfiguration.View);
        //private IView _view;

        /// <summary>
        /// This allows external settings to override the view given by the configuration. This is used to temporarily use an alternate view.
        /// For example, when previewing a different template. 
        /// </summary>
        public IView OverrideView { get; set; }


    }
}
