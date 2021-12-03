namespace ToSic.Sxc.Edit.Toolbar
{
    /// <summary>
    /// The toolbar builder helps you create Toolbar configurations for the UI.
    /// Note that it has a fluid API, and each method/use returns a fresh object with the updated configuration.
    /// </summary>
    /// <remarks>
    /// Your code cannot construct this object by itself, as it usually needs additional information.
    /// To get a `ToolbarBuilder`, use the <see cref="Services.IToolbarService"/>.
    /// </remarks>
    public interface IToolbarBuilder
    {
        /// <summary>
        /// Add one or more rules (as strings or ToolbarRule objects) according to the conventions of the [js toolbar](xref:JsCode.Toolbars.Simple)
        /// </summary>
        /// <param name="rules"></param>
        /// <returns></returns>
        ToolbarBuilder Add(params object[] rules);

        /// <summary>
        /// Create an add `metadata` rule to add or edit metadata to the specified object and using the content-type specified here. 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        ToolbarBuilder Metadata(object target, string contentType);
    }
}