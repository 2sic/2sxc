namespace ToSic.Sxc.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class SxcUiConstants
{
    /// <summary>
    /// Additional json-node for metadata in serialized entities, if user has edit rights
    /// </summary>
    internal const string JsonEntityEditNodeName = "_2sxcEditInformation";

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