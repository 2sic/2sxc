namespace ToSic.Sxc.Cms.Users;

[WorkInProgressApi("v20.01")]
public enum UserElevation
{
    /// <summary>
    /// Unknown state - should not be used, but is the default in case something is not specified.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// All elevations. This is used to set something which will apply to everybody.
    /// </summary>
    All = 1,

    /// <summary>
    /// Any elevation. This is used to set something which is applied to all which are not specified differently.
    /// </summary>
    Any = All + 1,

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

    /// <summary>
    /// Users with edit content-rights or higher.
    /// </summary>
    ContentEdit = 60,

    /// <summary>
    /// Users with content admin rights or higher.
    /// </summary>
    ContentAdmin = 70,

    /// <summary>
    /// Site admins - can do everything on a site.
    /// </summary>
    SiteAdmin = 80,

    /// <summary>
    /// System admins - can do everything on the entire system.
    /// </summary>
    SystemAdmin = 99,
}