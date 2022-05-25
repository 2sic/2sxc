using ToSic.Eav.Apps;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Sxc.Edit.Toolbar;
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
    [PrivateApi("Still WIP, not sure if this will be published as this - but probably we will")]
    public interface IToolbarBuilder: IHybridHtmlString, IHasLog
    {
        /// <summary>
        /// Add one or more rules according to the conventions of the [js toolbar](xref:JsCode.Toolbars.Simple)
        /// </summary>
        /// <param name="rules"></param>
        /// <returns></returns>
        /// <remarks>
        /// History
        /// * Added in 2sxc 13
        /// </remarks>
        IToolbarBuilder Add(params string[] rules);


        /// <summary>
        /// Add one or more rules (as strings or ToolbarRule objects) according to the conventions of the [js toolbar](xref:JsCode.Toolbars.Simple)
        /// </summary>
        /// <param name="rules"></param>
        /// <returns></returns>
        /// <remarks>
        /// History
        /// * Added in 2sxc 13
        /// </remarks>
        [PrivateApi("Would confuse people, since they cannot create ToolbarRule objects")]
        IToolbarBuilder Add(params object[] rules);

        /// <summary>
        /// Create an add `metadata` rule to add or edit metadata to the specified object and using the content-type specified here. 
        /// </summary>
        /// <param name="target">The target object which should receive metadata. Must support <see cref="ToSic.Eav.Metadata.IHasMetadata"/> </param>
        /// <param name="contentTypes">Name of one or more content-types for which to generate the button(s). For many, use comma `,` to separate.</param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="ui">Parameters for the UI, like color=red - see [toolbar docs](xref:JsCode.Toolbars.Simple) for all possible options</param>
        /// <param name="parameters">Parameters for the metadata-command</param>
        /// <returns></returns>
        /// <remarks>
        /// History
        /// * Added in 2sxc 13
        /// * parameter context added in 2sxc 14 - still WIP
        /// * contentTypes changed from one to many in v14
        /// * contentTypes can also have `*` or `SomeType,*` in v14
        /// </remarks>
        IToolbarBuilder Metadata(
            object target,
            string contentTypes,
            string noParamOrder = Eav.Parameters.Protector,
            string ui = null,
            string parameters = null,
            string context = null
        );

        [PrivateApi("WIP 13.11")]
        IToolbarBuilder Image(
            object target,
            string noParamOrder = Eav.Parameters.Protector,
            string ui = null,
            string parameters = null
        );


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
        /// <param name="parameters">Parameters for the command - doesn't really have an effect on Settings, but included for consistency</param>
        /// <returns></returns>
        /// <remarks>
        /// History
        /// * Added in 2sxc 13
        /// </remarks>
        IToolbarBuilder Settings(
            string noParamOrder = Eav.Parameters.Protector,
            string show = null,
            string hover = null,
            string follow = null,
            string classes = null,
            string autoAddMore = null,
            string ui = null,
            string parameters = null
        );

        /// <summary>
        /// Converts the configuration to a json-string according to the JS-Toolbar specs.
        /// </summary>
        /// <returns></returns>
        string ToString();


        [PrivateApi]
        IToolbarBuilder Init(IAppIdentity currentApp);

        [PrivateApi]
        ToolbarContext Context();
    }
}