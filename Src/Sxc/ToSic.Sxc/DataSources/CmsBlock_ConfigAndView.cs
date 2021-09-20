using ToSic.Eav.Plumbing;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;


namespace ToSic.Sxc.DataSources
{
    public sealed partial class CmsBlock
    {
        
        private BlockConfiguration BlockConfiguration => _blockConfiguration ?? (_blockConfiguration = LoadBlockConfiguration());
        private BlockConfiguration _blockConfiguration;

        private BlockConfiguration LoadBlockConfiguration()
        {
            var wrapLog = Log.Call<BlockConfiguration>();
            if (UseSxcInstanceContentGroup)
                return wrapLog("need content-group, will use from sxc-context", Block.Configuration);

            // If we don't have a context, then look it up based on the InstanceId
            Log.Add("need content-group, will construct as cannot use context");
            if (!ModuleId.HasValue)
            {
                SetError($"{nameof(CmsBlock)} cannot find Block Configuration", $"Neither InstanceContext nor {nameof(ModuleId)} found");
                return wrapLog("Error, no module-id", null);
            }

            var sp = DataSourceFactory.ServiceProvider;
            var userMayEdit = HasInstanceContext && Block.Context.UserMayEdit;

            var cms = _lazyCmsRuntime.IsValueCreated
                ? _lazyCmsRuntime.Value
                : _lazyCmsRuntime.Value.Init(this, userMayEdit, Log);
            var container = sp.Build<IModule>().Init(ModuleId.Value, Log);
            var blockId = container.BlockIdentifier;
            return wrapLog("ok", cms.Blocks.GetOrGeneratePreviewConfig(blockId));
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
