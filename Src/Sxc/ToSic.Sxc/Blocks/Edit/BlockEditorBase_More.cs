using System;
using ToSic.Eav.Data;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Blocks.Edit
{
    public partial class BlockEditorBase
    {
        internal static BlockEditorBase GetEditor(IBlock block)
        {
            if (block is BlockFromModule) return block.Context.ServiceProvider.Build<BlockEditorForModule>().Init(block);
            if (block is BlockFromEntity) return block.Context.ServiceProvider.Build<BlockEditorForEntity>().Init(block);
            throw new Exception("Can't find BlockEditor - the base block type in unknown");
        }

        // methods which the entity-implementation must customize - so it's virtual

        protected abstract void SavePreviewTemplateId(Guid templateGuid);

        internal abstract void SetAppId(int? appId);

        internal abstract void EnsureLinkToContentGroup(Guid cgGuid);

        internal abstract void UpdateTitle(IEntity titleItem);

    }
}
