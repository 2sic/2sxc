using ToSic.Eav.Metadata;
using ToSic.Lib.Data;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Context.Internal;

[PrivateApi("Hide implementation")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class CmsBlock(IBlock block) : Wrapper<IBlock>(block), ICmsBlock
{
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