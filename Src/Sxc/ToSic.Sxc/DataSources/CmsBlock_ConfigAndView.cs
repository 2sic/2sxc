using ToSic.Lib.Logging;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Blocks;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.DataSources
{
    public sealed partial class CmsBlock
    {
        
        private BlockConfiguration BlockConfiguration => _blockConfiguration ?? (_blockConfiguration = LoadBlockConfiguration());
        private BlockConfiguration _blockConfiguration;

        private BlockConfiguration LoadBlockConfiguration()
        {
            var wrapLog = Log.Fn<BlockConfiguration>();
            if (UseSxcInstanceContentGroup)
                return wrapLog.Return(Block.Configuration, "need content-group, will use from Sxc Instance ContentGroup");

            // If we don't have a context, then look it up based on the InstanceId
            Log.A("need content-group, will construct as cannot use context");
            if (!ModuleId.HasValue)
            {
                SetError($"{nameof(CmsBlock)} cannot find Block Configuration", $"Neither InstanceContext nor {nameof(ModuleId)} found");
                return wrapLog.ReturnNull("Error, no module-id");
            }

            var userMayEdit = HasInstanceContext && Block.Context.UserMayEdit;

            var cms = _lazyCmsRuntime.IsValueCreated
                ? _lazyCmsRuntime.Value
                : _lazyCmsRuntime.Value.Init(this, userMayEdit, Log);
            var container = _moduleLazy.Value.Init(ModuleId.Value, Log);
            var blockId = container.BlockIdentifier;
            return wrapLog.ReturnAsOk(cms.Blocks.GetOrGeneratePreviewConfig(blockId));
        }


        private IView View => _view ?? (_view = OverrideView ?? BlockConfiguration.View);
        private IView _view;

        /// <summary>
        /// This allows external settings to override the view given by the configuration. This is used to temporarily use an alternate view.
        /// For example, when previewing a different template. 
        /// </summary>
        public IView OverrideView { get; set; }


    }
}
