using ToSic.Eav.Metadata;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Context;

// Note: was WIP 13 till v17
[PublicApi]
public interface ICmsBlock: IHasMetadata
{
    /// <summary>
    /// The ID of this Block
    /// </summary>
    int Id { get; }

    /// <summary>
    /// Determines if this is the root block.
    /// Will be true in most cases, but false on inner-content
    /// </summary>
    bool IsRoot { get; }
}