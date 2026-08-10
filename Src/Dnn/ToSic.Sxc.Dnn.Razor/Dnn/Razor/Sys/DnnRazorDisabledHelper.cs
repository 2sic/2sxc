namespace ToSic.Sxc.Dnn.Razor.Sys;

/// <summary>
/// Trivial helper to place not-supported / disabled features and docs
/// </summary>
internal class DnnRazorDisabledHelper
{
    #region RenderPage

    /// <summary>
    /// RenderPage is disabled in Razor12+ to force designers to use Html.Partial
    /// </summary>
    internal static HelperResult RenderPageNotSupported()
        => throw new NotSupportedException("RenderPage(...) is not supported in Hybrid Razor. Use Html.Partial(...) instead.");


    #endregion

}
