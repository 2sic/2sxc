using System;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Run;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Blocks;

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
            if (!InstanceId.HasValue)
            {
                wrapLog("Error, no module-id", null);
                throw new Exception("Looking up BlockConfiguration failed because ModuleId is null.");
            }

            var sp = DataSourceFactory.ServiceProvider;
            var publish = sp.Build<IPagePublishingResolver>();//.Init(Log);
            var userMayEdit = HasInstanceContext && Block.EditAllowed;

            var cms = _lazyCmsRuntime.IsValueCreated
                ? _lazyCmsRuntime.Value
                : _lazyCmsRuntime.Value.Init(this, HasInstanceContext && userMayEdit,
                    publish.IsEnabled(InstanceId.Value), Log);
            var container = sp.Build<IContainer>().Init(InstanceId.Value, Log);
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
