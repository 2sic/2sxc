using ToSic.Lib.Documentation;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Adam
{
    /// <summary>
    /// Special interface - same as <see cref="IAsset"/> except that the metadata is typed.
    /// </summary>
    [WorkInProgressApi("Still WIP v16.02")]
    public interface ITypedAsset : IAsset, IHasMetadata<ITypedMetadata>
    {
    }
}
