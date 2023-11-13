using ToSic.Eav.Metadata;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Context
{
    [PrivateApi("WIP v13")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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
}
