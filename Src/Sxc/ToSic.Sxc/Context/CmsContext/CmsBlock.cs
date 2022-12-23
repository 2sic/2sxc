using ToSic.Eav.Data;
using ToSic.Eav.Metadata;
using ToSic.Lib.Data;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Blocks;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Context
{
    [PrivateApi("Hide implementation")]
    public class CmsBlock: Wrapper<IBlock>, ICmsBlock
    {
        public CmsBlock(IBlock block): base(block) { }

        /// <inheritdoc />
        public int Id => UnwrappedContents?.Configuration.Id ?? 0;

        /// <inheritdoc />
        public bool IsRoot => UnwrappedContents != null && UnwrappedContents.BlockBuilder.RootBuilder == UnwrappedContents.BlockBuilder;

        /// <inheritdoc />
        public IMetadataOf Metadata => UnwrappedContents.Configuration.Metadata;
    }
}
