namespace ToSic.Sxc.Data.Convert;

// 2025-07-04 2dm - experimental ideas for more sophisticated options
// on Children<T> etc. to choose which children can be shown.
// Not in use yet, also not clear if we would have an object with all these settings/options
// or have more parameters in the command.

/// <summary>
/// WIP
/// </summary>
public enum SelectPublishedOptions
{
    /// <summary>
    /// Automatic (default) shows draft-data to the editors and published data to visitors. Draft-only data is skipped.
    /// </summary>
    Auto = 0,

    /// <summary>
    /// No Draft means that only published data is used; drafts are filtered out (just like Auto for visitors).
    /// </summary>
    NoDraft = 1,

    /// <summary>
    /// Prefer Draft means that if data exists as draft, that will be preferred (just like Auto for Editors).
    /// </summary>
    PreferDraft = 2,

    ///// <summary>
    ///// Prefer Published means that published data is preferred, but that draft is also used if no published data is available.
    ///// </summary>
    ///// <remarks>
    ///// This is probably never used, since there is no clear use case where this makes sense.
    ///// </remarks>
    //PreferPublished,

}