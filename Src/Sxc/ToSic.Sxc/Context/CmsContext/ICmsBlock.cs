using ToSic.Eav.Metadata;

namespace ToSic.Sxc.Context;

/// <summary>
/// Information about the block - similar to a module.
/// The block is the 2sxc-internal definition of a unit of content.
/// In most cases each module has one block, but there are edge cases such as:
///
/// * modules which show the same block - different module-id, same block-id
/// * modules showing multiple blocks such as inner-content.
/// </summary>
/// <remarks>
/// Was added somewhere in 2sxc 13, but not documented/published till 2sxc 17.
/// </remarks>
[PublicApi]
public interface ICmsBlock: IHasMetadata
{
    /// <summary>
    /// The ID of this Block - corresponds to the EntityId in 2sxc which stores the block.
    /// </summary>
    /// <remarks>
    /// If exported and re-imported, this ID will change, so consider using the Guid instead.
    /// </remarks>
    int Id { get; }

    /// <summary>
    /// The Guid of this Block - corresponds to the EntityGuid in 2sxc which stores the block.
    /// </summary>
    /// <remarks>
    /// * Added in v17.08.
    /// * If exported and re-imported, this Guid will stay the same, so it's a better reference than the Id.
    /// </remarks>
    Guid Guid { get; }

    /// <summary>
    /// Determines if this is the root block, meaning it's the main block inside a module.
    /// Will be true in most cases, but false on inner-content
    /// </summary>
    bool IsRoot { get; }
}