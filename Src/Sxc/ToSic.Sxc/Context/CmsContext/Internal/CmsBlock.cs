using ToSic.Eav.Metadata;
using ToSic.Lib.Data;
using ToSic.Sxc.Blocks.Internal;

namespace ToSic.Sxc.Context.Internal;

internal class CmsBlock(IBlock block) : Wrapper<IBlock>(block), ICmsBlock
{
    /// <inheritdoc />
    public int Id => GetContents()?.Configuration.Id ?? 0;

    /// <inheritdoc />
    public Guid Guid => GetContents()?.Configuration.Guid ?? Guid.Empty;

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