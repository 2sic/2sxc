namespace ToSic.Sxc.Data;

/// <summary>
/// Publishing Information for <see cref="ITypedItem"/>s.
/// </summary>
/// <remarks>New v17</remarks>
[WorkInProgressApi("WIP v17")]
public interface IPublishing
{
    /// <summary>
    /// Informs you if the current Item support publishing.
    /// Basically all real Items based on IEntity support publishing, but in some cases you will have
    /// <see cref="ITypedItem"/>s which are not based on an entity, and those will not support publishing.
    ///
    /// By default, those objects will say `IsPublished` == `true`, `HasPublished` == `true` and `HasUnpublished` == `false`.
    /// </summary>
    bool IsSupported { get; }

    /// <summary>
    /// True if this item has a published version.
    /// Note that this is also true if the current item is the published version.
    /// </summary>
    bool HasPublished { get; }

    /// <summary>
    /// True if this item has an unpublished version.
    /// Note that this is also true if the current item is the unpublished version.
    /// </summary>
    bool HasUnpublished { get; }

    /// <summary>
    /// True if this item **branches** meaning it has a published version _and_ an unpublished draft version.
    /// </summary>
    bool HasBoth { get; }

    /// <summary>
    /// Get the published version of this item.
    /// If the initial item was already published, it will return that item.
    /// </summary>
    /// <returns>the published item or `null`</returns>
    ITypedItem GetPublished();

    /// <summary>
    /// Get the unpublished version of this item.
    /// If the initial item was already unpublished, it will return that item.
    /// </summary>
    /// <returns>the unpublished item or `null`</returns>
    ITypedItem GetUnpublished();

    /// <summary>
    /// Get the opposite version of this item.
    /// So if your initial item was published, it will try to get the unpublished, and vice versa.
    /// </summary>
    /// <returns>the other version of this item or `null`</returns>
    ITypedItem GetOpposite();

}