namespace ToSic.Sxc.Edit.Toolbar
{

    public partial interface IToolbarBuilder
    {
        /// <summary>
        /// Add a `more` button. Not really useful to do, but included for completeness
        /// </summary>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="ui"></param>
        /// <returns></returns>
        IToolbarBuilder More(
            string noParamOrder = Eav.Parameters.Protector,
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
            string noParamOrder = Eav.Parameters.Protector,
            string show = null,
            string hover = null,
            string follow = null,
            string classes = null,
            string autoAddMore = null,
            object ui = null,
            object parameters = null
        );

        /// <summary>
        /// Adds / updates the `params` rule on the toolbar which contains information for all the buttons
        /// </summary>
        /// <param name="target">
        /// Many options
        /// 1. An Entity-like thing which would be used to prepare default params like `entityId`
        /// 1. A string, which would be the same as using the term on the `parameters`
        /// 1. A object - especially an anonymous object like `new { id = 7, show = true }`
        /// </param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="ui"></param>
        /// <param name="parameters"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        IToolbarBuilder Parameters(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null,
            object prefill = null,
            string context = null
        );

        
    }
}