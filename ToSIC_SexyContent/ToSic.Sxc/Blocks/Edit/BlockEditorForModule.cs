using System;
using ToSic.Eav;
using ToSic.Eav.Data;
using ToSic.Sxc.Interfaces;
using ToSic.Sxc.Run;

namespace ToSic.Sxc.Blocks.Edit
{
    internal class BlockEditorForModule: BlockEditorBase
    {
        protected override void SavePreviewTemplateId(Guid templateGuid)
            => Factory.Resolve<IEnvironmentConnector>().SetPreview(ModuleId, templateGuid);

        internal override void SetAppId(int? appId)
            => Factory.Resolve<IEnvironmentConnector>().SetAppId(BlockBuilder.Container, BlockBuilder.Environment, appId, Log);

        internal override void EnsureLinkToContentGroup(Guid cgGuid)
            => Factory.Resolve<IEnvironmentConnector>().SetContentGroup(ModuleId, true, cgGuid);

        internal override void UpdateTitle(IEntity titleItem)
        {
            Log.Add("update title");
            Factory.Resolve<IEnvironmentConnector>().UpdateTitle(BlockBuilder, titleItem);
        }
    }
}