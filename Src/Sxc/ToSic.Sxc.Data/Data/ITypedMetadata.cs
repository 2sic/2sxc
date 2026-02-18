using ToSic.Eav.Data.Sys.Entities;
using ToSic.Eav.Metadata;

namespace ToSic.Sxc.Data;

/// <summary>
/// Metadata on Dynamic Objects - like <see cref="IDynamicEntity"/> or <see cref="ToSic.Sxc.Adam.IAsset"/> (files/folders).
/// </summary>
/// <remarks>
/// Behaves like a normal DynamicEntity, but has additional commands to detect if specific Metadata exists.
/// 
/// History:
/// 
/// * Added in v13
/// * Made compatible to <see cref="ITypedItem"/> in 16.02 to allow typed commands such as `.String(...)`
/// * Renamed in v16.02 from `IDynamicMetadata` to `IMetadata` since it's not necessarily `dynamic` any more (but still supports `dynamic` where needed)
///     _Note that this is a breaking change, but we believe the type is never directly mentioned in any code_
/// * Renamed in v20 to `ITypedMetadata` from `IMetadata` because it kept on causing confusions
/// </remarks>
[InternalApi_DoNotUse_MayChangeWithoutNotice("The name can change, but the APIs are safe to use.")]
public interface ITypedMetadata: /*IHasMetadata, */ITypedItem, ICanDebug, /*ISxcDynamicObject,*/ IEntityWrapper
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
    /// WIP v21.02
    /// </summary>
    /// <returns></returns>
    IEnumerable<IEntity> GetAll();

    // TODO: REMOVE
    ///// <summary>
    ///// Old property for the ID of the first type.
    ///// It was necessary to re-instate this because it's used in old Apps such as BlueImp Gallery.
    ///// </summary>
    //[PrivateApi]
    //[ShowApiWhenReleased(ShowApiMode.Never)]
    //int EntityId { get; }
}

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