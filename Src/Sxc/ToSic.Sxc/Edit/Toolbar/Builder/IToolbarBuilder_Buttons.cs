using ToSic.Eav;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial interface IToolbarBuilder
    {
        /// <summary>
        /// Add a custom button / command. 
        /// </summary>
        /// <param name="name">
        /// 1. The _required_ name of the command.
        /// See [](xref:Api.Js.SxcJs.CommandNames).
        /// 
        /// 2. Can also be a full rule-string containing parameters and more
        /// according to the conventions of the [js toolbar](xref:JsCode.Toolbars.Simple)
        /// </param>
        /// <param name="target">Optional entity-like target. If provided, will include the specs of that entity in the parameters. </param>
        /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
        /// <param name="ui"></param>
        /// <param name="parameters"></param>
        /// <param name="operation">see <see cref="ToolbarRuleOperation"/></param>
        /// <param name="context"></param>
        /// <returns></returns>
        IToolbarBuilder Button(
            string name,
            object target = null,
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null,
            string context = null
        );

    }
}