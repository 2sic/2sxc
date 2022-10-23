using ToSic.Eav.Logging;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Code;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Edit.Toolbar
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
    /// * Added in 2sxc 13, just minimal API
    /// * massively enhanced in v14.04
    /// </remarks>
    [PublicApi]
    public partial interface IToolbarBuilder: IHybridHtmlString, IHasLog, INeedsDynamicCodeRoot
    {
        [PrivateApi("internal use only")]
        IToolbarBuilder Toolbar(
            string toolbarTemplate,
            object target = null,
            object ui = null,
            object parameters = null,
            object prefill = null
        );

        // 2022-08-20 #cleanUpImageToolbar
        //[PrivateApi("WIP 13.11 - not sure if we actually make it public, as it's basically metadata with automatic content-type - not published yet")]
        //IToolbarBuilder Image(
        //    object target,
        //    string noParamOrder = Eav.Parameters.Protector,
        //    string ui = null,
        //    string parameters = null
        //);



    }
}