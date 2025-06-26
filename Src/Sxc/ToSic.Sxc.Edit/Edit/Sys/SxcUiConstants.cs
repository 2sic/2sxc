namespace ToSic.Sxc.Internal;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class SxcUiConstants
{
    /// <summary>
    /// Wrapper tag which contains the context information.
    /// Usually just used in edit mode, but in rare cases also at runtime
    /// </summary>
    internal const string DefaultContextTag = "div";

    /// <summary>
    /// Decorator crass to mark a content-block in the HTML
    /// </summary>
    internal const string ClassToMarkContentBlock = "sc-content-block";
}