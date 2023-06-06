using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Data
{
    /// <summary>
    /// Objects which have metadata - usually a <see cref="ITypedItem"/>, <see cref="IDynamicEntity"/>, <see cref="Adam.IFile"/> etc.
    /// </summary>
    /// <typeparam name="TMetadata">The type, either classic <see cref="IDynamicMetadata"/> or <see cref="ITypedMetadata"/></typeparam>
    /// <remarks>Added in v16.02</remarks>
    [PublicApi]
    public interface IHasMetadata<out TMetadata>
    {
        /// <summary>
        /// List of metadata items - 
        /// will automatically contain a fake item, even if no metadata exits
        /// to help in razor template etc.
        /// </summary>
        /// <returns>An Metadata-like object which contains the metadata, or an empty similar object which still works if no metadata exists.</returns>
        TMetadata Metadata { get; }

    }
}
