namespace ToSic.Sxc.Compatibility.RazorPermissions;

/// <summary>
/// This is a compatibility leftover from old code - new code uses Edit.Enabled
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class RazorPermissions
{
    internal RazorPermissions(bool editAllowed) => UserMayEditContent = editAllowed;

    /// <summary>
    /// This property is used publicly, so it must exist
    /// </summary>
    // ReSharper disable once UnusedMember.Global
    public bool UserMayEditContent { get; }
}