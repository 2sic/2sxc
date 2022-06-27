using ToSic.Eav;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial interface IToolbarBuilder
    {

        /// <summary>
        /// Add one or more rules according to the conventions of the [js toolbar](xref:JsCode.Toolbars.Simple).
        ///
        /// Note that you can actually add many buttons but the name is still Button not Buttons, for API consistency.
        /// </summary>
        /// <param name="rules"></param>
        /// <returns></returns>
        /// <remarks>
        /// History
        /// - new in 14.04 WIP
        /// </remarks>
        IToolbarBuilder ButtonAdd(params string[] rules);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">
        /// The command name.
        /// See [](xref:Api.Js.SxcJs.CommandNames)
        /// </param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="ui">
        /// Parameters for the UI, like `color=red` - see [toolbar docs](xref:JsCode.Toolbars.Simple) for all possible options.
        /// Can be a string, and can also be an object since v14.04
        /// </param>
        /// <param name="parameters">
        /// Parameters for the command.
        /// Can be a string, and can also be an object since v14.04
        /// </param>
        /// <returns></returns>
        /// <remarks>
        /// - new in 14.04 WIP
        /// </remarks>
        [WorkInProgressApi("still WIP")]
        IToolbarBuilder ButtonModify(
            string name,
            string noParamOrder = Parameters.Protector,
            //object target = null,
            object ui = null,
            object parameters = null);

        /// <summary>
        /// Remove buttons from the toolbar.
        /// Usually in combination with the `Default` toolbar which already has many buttons, for eg to remove the `layout` button.
        ///
        /// Note that you can actually remove many buttons but the name is still Button not Buttons, for API consistency.
        /// </summary>
        /// <param name="names">
        /// One or more command names.
        /// See [](xref:Api.Js.SxcJs.CommandNames)
        /// </param>
        /// <returns></returns>
        /// <remarks>
        /// - new in 14.04 WIP
        /// </remarks>
        [WorkInProgressApi("still WIP")]
        IToolbarBuilder ButtonRemove(params string[] names);
        
    }
}