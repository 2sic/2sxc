using System;
using ToSic.Eav.Data;
using ToSic.Eav.DI;
using ToSic.Lib.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Run;

namespace ToSic.Sxc.Blocks.Edit
{
    public class BlockEditorForModule : BlockEditorBase
    {
        public BlockEditorForModule(BlockEditorBaseDependencies dependencies,
            LazyInitLog<IPlatformModuleUpdater> platformModuleUpdater) : base(dependencies)
        {
            _platformModuleUpdater = platformModuleUpdater.SetLog(Log);
        }

        private readonly LazyInitLog<IPlatformModuleUpdater> _platformModuleUpdater;

        private IPlatformModuleUpdater PlatformModuleUpdater => _platformModuleUpdater.Value;


        protected override void SavePreviewTemplateId(Guid templateGuid)
            => PlatformModuleUpdater.SetPreview(Block.Context.Module.Id, templateGuid);


        internal override void SetAppId(int? appId)
            => PlatformModuleUpdater.SetAppId(Block.Context.Module, appId);

        internal override void EnsureLinkToContentGroup(Guid cgGuid)
            => PlatformModuleUpdater.SetContentGroup(Block.Context.Module.Id, true, cgGuid);

        internal override void UpdateTitle(IEntity titleItem)
        {
            Log.A("update title");
            PlatformModuleUpdater.UpdateTitle(Block, titleItem);
        }

    }
}