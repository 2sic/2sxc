namespace ToSic.Sxc.Edit.Toolbar;

public partial interface IToolbarBuilder
{
    /// <summary>
    /// Add a custom button / command.
    /// Can also be used to do advanced remove operations or modify a button on a toolbar which would have it by default. 
    /// </summary>
    /// <param name="name">
    /// 1. The _required_ name of the command.
    /// See [](xref:Api.Js.SxcJs.CommandNames).
    /// 
    /// 2. Can also be a full rule-string containing parameters and more
    /// according to the conventions of the [js toolbar](xref:JsCode.Toolbars.Simple)
    /// </param>
    /// <param name="target">_optional_ entity-like target, see [target guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Target)</param>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="tweak">Functional [Tweak API](xref:ToSic.Sxc.Services.ToolbarBuilder.TweakButtons) to modify UI and parameters</param>
    /// <param name="ui">_optional_ configuration how to show, see [ui guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Ui)</param>
    /// <param name="parameters">_optional_ parameters for the command, see [parameters guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Parameters)</param>
    /// <param name="operation">_optional_ change [what should happen](xref:ToSic.Sxc.Services.ToolbarBuilder.Operation)</param>
    /// <param name="context"></param>
    /// <returns>a _new_ toolbar builder - see [guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Index)</returns>
    IToolbarBuilder Button(
        string name,
        object target = null,
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton> tweak = default,
        object ui = null,
        object parameters = null,
        string operation = null,
        string context = null
    );

}