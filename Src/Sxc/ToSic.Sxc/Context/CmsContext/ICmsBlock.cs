using ToSic.Eav.Documentation;
using ToSic.Eav.Metadata;

namespace ToSic.Sxc.Context
{
    [PrivateApi("WIP v13")]
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
