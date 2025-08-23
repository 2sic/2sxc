namespace ToSic.Sxc.Cms.Users;

/// <summary>
/// Describes the elevation of a user, which is a simplified way to describe permissions.
/// </summary>
[WorkInProgressApi("v20.01")]
public enum UserElevation
{
    /// <summary>
    /// Unknown state - should not be used, but is the default in case something is not specified.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Any elevation. This is used to set something which is applied to all which are not specified differently.
    /// </summary>
    All = 1,

    /// <summary>
    /// Anonymous users (not logged in).
    /// </summary>
    Anonymous = 10,

    /// <summary>
    /// Users with view rights - usually all logged-in users.
    /// </summary>
    View = 20,

    // CreateDraft = 3,

    // EditDraft = 4,

    // Create = 5,
    ContentDraft = 50,

    /// <summary>
    /// Users with edit content-rights or higher.
    /// </summary>
    ContentEdit = ContentDraft + 1,

    /// <summary>
    /// Users with content admin rights or higher.
    /// </summary>
    ContentAdmin = ContentEdit + 1,

    /// <summary>
    /// Site admins - can do everything on a site.
    /// </summary>
    SiteAdmin = 80,

    /// <summary>
    /// System admins - can do everything on the entire system.
    /// </summary>
    SystemAdmin = 90,
}