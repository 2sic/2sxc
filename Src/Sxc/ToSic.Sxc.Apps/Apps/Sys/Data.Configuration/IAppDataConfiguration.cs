using ToSic.Lib.LookUp.Engines;

namespace ToSic.Eav.Apps.Internal;

/// <summary>
/// The configuration of an app-data - usually relevant so the source will auto-filter out unpublished data for normal viewers.
/// </summary>
[PrivateApi("before v17 was InternalApi_DoNotUse_MayChangeWithoutNotice - this is just fyi")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IAppDataConfiguration
{
    /// <summary>
    /// If this instance is allowed to show draft items
    /// This is usually dependent on the current users permissions
    /// </summary>
    bool? ShowDrafts { get; }


    /// <summary>
    /// Configuration used to query data - will deliver url-parameters and other important configuration values.
    /// </summary>
    ILookUpEngine Configuration { get; }
}