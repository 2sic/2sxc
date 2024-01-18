namespace ToSic.Sxc.Web.Internal.PageService;

[PrivateApi("not final yet, probably will not be implemented like this")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public enum PageChangeModes
{
    /// <summary>
    /// Default - similar to Auto
    /// </summary>
    Default,
        
    /// <summary>
    /// Auto - so a change will be applied as best seen fit
    /// </summary>
    Auto,
        
    /// <summary>
    /// Replace the original implementation.
    /// </summary>
    Replace,
        
    /// <summary>
    /// Attempt to replace, otherwise don't apply the change.
    /// </summary>
    ReplaceOrSkip,
        
    /// <summary>
    /// Append the change.
    /// </summary>
    Append,
        
    /// <summary>
    /// Prepend the change. 
    /// </summary>
    Prepend,
}