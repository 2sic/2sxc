using ToSic.Eav.Metadata;

namespace ToSic.Sxc.Data;

/// <summary>
/// Metadata on Dynamic Objects - like <see cref="IDynamicEntity"/> or <see cref="ToSic.Sxc.Adam.IAsset"/> (files/folders).
/// 
/// Behaves like a normal DynamicEntity, but has additional commands to detect if specific Metadata exists.
/// </summary>
/// <remarks>
/// * Added in v13
/// * Made compatible to <see cref="ITypedItem"/> in 16.02 to allow typed commands such as `.String(...)`
/// * Renamed in v16.02 from `IDynamicMetadata` to `IMetadata` since it's not necessarily `dynamic` any more (but still supports `dynamic` where needed)
///     _Note that this is a breaking change, but we believe the type is never directly mentioned in any code_
/// </remarks>
[PublicApi]
public interface IMetadata: /*IDynamicEntity,*/ IHasMetadata, ITypedItem, ICanDebug, ISxcDynamicObject, IEntityWrapper
{
    /// <summary>
    /// Ask if there is metadata of the type specified.
    /// This is important in scenarios where an item could have a lot of metadata, but we only want one specific type to look at.
    /// </summary>
    /// <param name="type"></param>
    /// <returns>`true` if metadata of that type exists</returns>
    // ReSharper disable once UnusedMember.Global
    bool HasType(string type);

    /// <summary>
    /// Get all the metadata Entities of a specific type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    IEnumerable<IEntity> OfType(string type);

    /// <summary>
    /// Old property for the ID of the first type.
    /// It was necessary to re-instate this because it's used in old Apps such as BlueImp Gallery.
    /// </summary>
    [PrivateApi]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    int EntityId { get; }
}