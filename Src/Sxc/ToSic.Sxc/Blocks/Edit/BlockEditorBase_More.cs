using System;
using ToSic.Eav.Data;

namespace ToSic.Sxc.Blocks.Edit
{
    public partial class BlockEditorBase
    {
        internal static BlockEditorBase GetEditor(IBlock block)
        {
            if (block is BlockFromModule) return new BlockEditorForModule().Init(block);
            if (block is BlockFromEntity) return new BlockEditorForEntity().Init(block);
            throw new Exception("Can't find BlockEditor - the base block type in unknown");
        }

        // methods which the entity-implementation must customize - so it's virtual

        protected abstract void SavePreviewTemplateId(Guid templateGuid);

        internal abstract void SetAppId(int? appId);

        internal abstract void EnsureLinkToContentGroup(Guid cgGuid);

        internal abstract void UpdateTitle(IEntity titleItem);

    }
}
