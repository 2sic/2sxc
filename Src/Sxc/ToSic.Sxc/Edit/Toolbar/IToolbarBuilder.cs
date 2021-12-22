using ToSic.Eav.Documentation;
using ToSic.Sxc.Web;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Edit
{
    /// <summary>
    /// The toolbar builder helps you create Toolbar configurations for the UI.
    /// Note that it has a fluid API, and each method/use returns a fresh object with the updated configuration.
    /// </summary>
    /// <remarks>
    /// Your code cannot construct this object by itself, as it usually needs additional information.
    /// To get a `ToolbarBuilder`, use the <see cref="Services.IToolbarService"/>.
    ///
    /// History
    /// * Added in 2sxc 13
    /// </remarks>
    [PublicApi("WIP v13")]
    public interface IToolbarBuilder: IHybridHtmlString
    {
        /// <summary>
        /// Add one or more rules according to the conventions of the [js toolbar](xref:JsCode.Toolbars.Simple)
        /// </summary>
        /// <param name="rules"></param>
        /// <returns></returns>
        IToolbarBuilder Add(params string[] rules);


        /// <summary>
        /// Add one or more rules (as strings or ToolbarRule objects) according to the conventions of the [js toolbar](xref:JsCode.Toolbars.Simple)
        /// </summary>
        /// <param name="rules"></param>
        /// <returns></returns>
        [PrivateApi("Would confuse people, since they cannot create ToolbarRule objects")]
        IToolbarBuilder Add(params object[] rules);

        /// <summary>
        /// Create an add `metadata` rule to add or edit metadata to the specified object and using the content-type specified here. 
        /// </summary>
        /// <param name="target">The target object which should receive metadata. Must support <see cref="ToSic.Eav.Metadata.IHasMetadata"/> </param>
        /// <param name="contentType"></param>
        /// <returns></returns>
        IToolbarBuilder Metadata(
            object target, 
            string contentType);

        /// <summary>
        /// Add a `settings` rule to configure what the toolbar should look like. See [](xref:JsCode.Toolbars.Settings)
        /// </summary>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="show"></param>
        /// <param name="hover"></param>
        /// <param name="follow"></param>
        /// <param name="classes"></param>
        /// <param name="autoAddMore"></param>
        /// <param name="ui">Parameters for the UI, like color=red - see [toolbar docs](xref:JsCode.Toolbars.Simple) for all possible options</param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IToolbarBuilder Settings(
            string noParamOrder = Eav.Parameters.Protector,
            string show = null,
            string hover = null,
            string follow = null,
            string classes = null,
            string autoAddMore = null,
            string ui = "", 
            string parameters = "");

        /// <summary>
        /// Converts the configuration to a json-string according to the JS-Toolbar specs.
        /// </summary>
        /// <returns></returns>
        string ToString();
    }
}