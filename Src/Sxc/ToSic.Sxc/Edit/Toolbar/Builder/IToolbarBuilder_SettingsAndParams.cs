namespace ToSic.Sxc.Edit.Toolbar;

public partial interface IToolbarBuilder
{
    /// <summary>
    /// Add a `more` button. Not really useful to do, but included for completeness
    /// </summary>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="ui">_optional_ configuration how to show, see [ui guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Ui)</param>
    /// <returns>a _new_ toolbar builder - see [guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Index)</returns>
    IToolbarBuilder More(
        NoParamOrder noParamOrder = default,
        object ui = default
    );

    /// <summary>
    /// Add a `settings` rule to configure what the toolbar should look like. See [](xref:JsCode.Toolbars.Settings)
    /// </summary>
    /// <param name="noParamOrder">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="show">`hover` (default), `always`</param>
    /// <param name="hover">`right` (default), `left`</param>
    /// <param name="follow"></param>
    /// <param name="classes">css class names to add to the entire toolbar</param>
    /// <param name="autoAddMore">placement of the _more_ ellipsis button, `auto` (default), `end`, `start`, `never`</param>
    /// <param name="ui">_optional_ configuration how to show, see [ui guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Ui)</param>
    /// <param name="parameters">Parameters for the command - doesn't really have an effect on Settings, but included for consistency</param>
    /// <returns>a _new_ toolbar builder - see [guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Index)</returns>
    /// <remarks>
    /// History
    /// * Added in 2sxc 13
    /// * options `auto` and `never` on autoAddMore added in 14.08
    /// </remarks>
    IToolbarBuilder Settings(
        NoParamOrder noParamOrder = default,
        string show = default,
        string hover = default,
        string follow = default,
        string classes = default,
        string autoAddMore = default,
        object ui = default,
        object parameters = default
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
    /// <param name="ui">_optional_ configuration how to show, see [ui guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Ui)</param>
    /// <param name="tweak">Functional [Tweak API](xref:ToSic.Sxc.Services.ToolbarBuilder.TweakButtons) to modify UI and parameters (new v16.02)</param>
    /// <param name="parameters">_optional_ parameters for the command, see [parameters guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Parameters)</param>
    /// <param name="prefill">_optional_ prefill for the edit-UI, see [prefill guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Prefill)</param>
    /// <param name="context"></param>
    /// <returns>a _new_ toolbar builder - see [guide](xref:ToSic.Sxc.Services.ToolbarBuilder.Index)</returns>
    IToolbarBuilder Parameters(
        object target = default,
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton> tweak = default,
        object ui = default,
        object parameters = default,
        object prefill = default,
        string context = default
    );

        
}