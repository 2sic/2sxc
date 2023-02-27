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
        public int Id => GetContents()?.Configuration.Id ?? 0;

        /// <inheritdoc />
        public bool IsRoot
        {
            get
            {
                var contents = GetContents();
                return contents != null && contents.BlockBuilder.RootBuilder == contents.BlockBuilder;
            }
        }

        /// <inheritdoc />
        public IMetadataOf Metadata => GetContents().Configuration.Metadata;
    }
}
