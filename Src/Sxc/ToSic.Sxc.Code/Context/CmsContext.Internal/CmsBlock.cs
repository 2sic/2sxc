using ToSic.Eav.Metadata;
using ToSic.Sxc.Blocks.Internal;

namespace ToSic.Sxc.Context.Internal;

internal class CmsBlock(IBlock block) : ICmsBlock
{
    /// <inheritdoc />
    public int Id => block?.Configuration.Id ?? 0;

    /// <inheritdoc />
    public Guid Guid => block?.Configuration.Guid ?? Guid.Empty;

    /// <inheritdoc />
    public bool IsRoot => block is { ParentBlockOrNull: null };

    /// <inheritdoc />
    public IMetadataOf Metadata => block.Configuration.Metadata;
}