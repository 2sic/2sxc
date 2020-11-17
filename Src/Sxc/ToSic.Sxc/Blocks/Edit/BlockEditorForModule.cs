using System;
using ToSic.Eav;
using ToSic.Eav.Data;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Run;

namespace ToSic.Sxc.Blocks.Edit
{
    internal class BlockEditorForModule: BlockEditorBase
    {
        public BlockEditorForModule(IServiceProvider serviceProvider, Lazy<CmsRuntime> lazyCmsRuntime, Lazy<CmsManager> cmsManagerLazy) : base(serviceProvider, lazyCmsRuntime, cmsManagerLazy) { }
        private IPlatformModuleUpdater PlatformModuleUpdater => _platformModuleUpdater 
                                                                ?? (_platformModuleUpdater = ServiceProvider.Build<IPlatformModuleUpdater>().Init(Log));
        private IPlatformModuleUpdater _platformModuleUpdater;

        protected override void SavePreviewTemplateId(Guid templateGuid)
            => PlatformModuleUpdater.SetPreview(Block.Context.Container.Id, templateGuid);


        internal override void SetAppId(int? appId)
            => PlatformModuleUpdater.SetAppId(Block.Context.Container, appId);

        internal override void EnsureLinkToContentGroup(Guid cgGuid)
            => PlatformModuleUpdater.SetContentGroup(Block.Context.Container.Id, true, cgGuid);

        internal override void UpdateTitle(IEntity titleItem)
        {
            Log.Add("update title");
            PlatformModuleUpdater.UpdateTitle(Block, titleItem);
        }

    }
}