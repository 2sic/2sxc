namespace ToSic.Sxc.Data;

/// <summary>
/// Experimental 2dm - not done yet 2024-08
/// Idea is to provide versioning information for items - different for original item, latest item, etc.
///
/// See IVersion / ILifecycle - not yet in use.
/// </summary>
internal interface IVersion
{
    /// <summary>
    /// The version, starting at 1 when the item is created.
    /// Future draft versions may not have a number yet, specs still missing.
    /// </summary>
    public int Number { get; }

    ///// <summary>
    ///// Info if this is a draft or published version.
    ///// </summary>
    //public bool IsPublished { get; }

    /// <summary>
    /// The date when this version was created.
    /// </summary>
    public DateTime Created { get; }

    /// <summary>
    /// The user who created / modified this version.
    /// </summary>
    public int UserId { get; }

    public int OwnerId { get; } // ??
}