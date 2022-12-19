using System;
using ToSic.Eav.Data;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;

namespace ToSic.Sxc.Blocks.Edit
{
    public partial class BlockEditorBase
    {
        internal static BlockEditorBase GetEditor(IBlock block, 
            Generator<BlockEditorForModule> blkEdtForMod,
            Generator<BlockEditorForEntity> blkEdtForEnt)
        {
            if (block is BlockFromModule) return blkEdtForMod.New().Init(block);
            if (block is BlockFromEntity) return blkEdtForEnt.New().Init(block);
            throw new Exception("Can't find BlockEditor - the base block type in unknown");
        }

        // methods which the entity-implementation must customize - so it's virtual

        protected abstract void SavePreviewTemplateId(Guid templateGuid);

        internal abstract void SetAppId(int? appId);

        internal abstract void EnsureLinkToContentGroup(Guid cgGuid);

        internal abstract void UpdateTitle(IEntity titleItem);

    }
}
