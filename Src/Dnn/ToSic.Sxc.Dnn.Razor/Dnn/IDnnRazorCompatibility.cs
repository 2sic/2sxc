namespace ToSic.Sxc.Dnn;

/// <summary>
/// Modify any objects on DNN Razor to match the newer .net core conventions.
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IDnnRazorCompatibility
{
    /// <summary>
    /// Helper for Html.Raw - for creating raw html output which doesn't encode &gt; and &lt;.
    /// Also has helpers such as `.Partial(...)`
    /// </summary>
    IHtmlHelper Html { get; }

}