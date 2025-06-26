namespace ToSic.Sxc.Data;

/// <summary>
/// Experimental 2dm - not done yet 2024-08
/// 2dm Experimental - trying to get the lifecycle info into ITypedItem
/// 
/// Idea is to provide versioning information for items - different for original item, latest item, etc.
///
/// See IVersion / ILifecycle - not yet in use.
/// </summary>
[PrivateApi]
public interface ILifecycle
{
    /// <summary>
    /// The version, starting at 1 when the item is created.
    /// Future draft versions may not have a number yet, specs still missing.
    /// </summary>
    int Version { get; }

    /// <summary>
    /// The date when this thing was created.
    /// </summary>
    DateTime Created { get; }
    //IUserModel CreatedUser { get; }
    ///// <summary>
    ///// The user who initially created this item.
    ///// </summary>
    //int CreatedUserId { get; }

    /// <summary>
    /// The date when this thing was last modified.
    /// </summary>
    DateTime Modified { get; }
    //IUserModel ModifiedUser { get; }
    ///// <summary>
    ///// The user who modified this version.
    ///// </summary>
    //int ModifiedUserId { get; }

    /// <summary>
    /// The user Id of the owner of this thing.
    /// </summary>
    /// <returns>The User Id or -1 if unknown.</returns>
    int OwnerId { get; }
}