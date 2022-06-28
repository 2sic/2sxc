using ToSic.Eav;

namespace ToSic.Sxc.Edit.Toolbar
{

    public partial interface IToolbarBuilder
    {
        IToolbarBuilder More(
            string noParamOrder = Parameters.Protector,
            object ui = null
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
        /// <param name="ui">
        /// Parameters for the UI, like `color=red` - see [toolbar docs](xref:JsCode.Toolbars.Simple) for all possible options.
        /// Can be a string, and can also be an object since v14.04
        /// </param>
        /// <param name="parameters">Parameters for the command - doesn't really have an effect on Settings, but included for consistency</param>
        /// <returns>A new toolbar builder which has been extended with this settings-rule</returns>
        /// <remarks>
        /// History
        /// * Added in 2sxc 13
        /// </remarks>
        IToolbarBuilder Settings(
            string noParamOrder = Parameters.Protector,
            string show = null,
            string hover = null,
            string follow = null,
            string classes = null,
            string autoAddMore = null,
            object ui = null,
            object parameters = null
        );
        
        
        // TODO: Params

    }
}