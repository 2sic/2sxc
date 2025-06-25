using ToSic.Eav.Metadata;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Blocks.Sys;

namespace ToSic.Sxc.Context.Internal;

internal class CmsBlock(IBlock block) : ICmsBlock
{
    /// <inheritdoc />
    public int Id => block.ConfigurationIsReady
        ? block.Configuration.Id
        : 0;

    /// <inheritdoc />
    public Guid Guid => block.ConfigurationIsReady
        ? block.Configuration.Guid
        : Guid.Empty;

    /// <inheritdoc />
    public bool IsRoot => block is { ParentBlockOrNull: null };

    /// <inheritdoc />
    public IMetadata Metadata => block.Configuration.Metadata;
}