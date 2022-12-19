using ToSic.Eav.Data;
using ToSic.Eav.Metadata;
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
        public int Id => _contents?.Configuration.Id ?? 0;

        /// <inheritdoc />
        public bool IsRoot => _contents != null && _contents.BlockBuilder.RootBuilder == _contents.BlockBuilder;

        /// <inheritdoc />
        public IMetadataOf Metadata => _contents.Configuration.Metadata;
    }
}
