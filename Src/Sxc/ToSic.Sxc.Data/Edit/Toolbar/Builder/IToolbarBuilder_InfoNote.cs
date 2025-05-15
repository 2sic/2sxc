namespace ToSic.Sxc.Edit.Toolbar;

public partial interface IToolbarBuilder
{
    /// <summary>
    /// Create an info, warning, help or link-button to assist the user.
    /// </summary>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="target">the target window, like `_blank` - new in v17; defaults to `null` so that ctrl-click etc. work as expected</param>
    /// <param name="tweak">Optional function call to tweak the button.</param>
    /// <param name="link">If provided, will make the button open the link in a new window.</param>
    /// <returns></returns>
    /// <remarks>
    /// * Added in v15.07
    /// * `target` added in v17 - defaults to nothing, so if you explicitly want a new window, you must set it to `_blank`
    /// </remarks>
    IToolbarBuilder Info(
        NoParamOrder noParamOrder = default,
        string link = default,
        string target = default,
        Func<ITweakButton, ITweakButton> tweak = default
    );
}