using ToSic.Eav.Data.Sys.Entities;
using ToSic.Eav.Metadata;

namespace ToSic.Sxc.Data;

[PrivateApi("old, should not be promoted any more")]
public interface IMetadataDynamic : IHasMetadata, ICanDebug, ISxcDynamicObject, IEntityWrapper
{
    /// <inheritdoc cref="ITypedMetadata.HasType"/>
    bool HasType(string type);

    /// <inheritdoc cref="ITypedMetadata.OfType"/>
    IEnumerable<IEntity> OfType(string type);

    /// <summary>
    /// Old property for the ID of the first type.
    /// It was necessary to re-instate this because it's used in old Apps such as BlueImp Gallery.
    /// </summary>
    [PrivateApi]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    int EntityId { get; }
}