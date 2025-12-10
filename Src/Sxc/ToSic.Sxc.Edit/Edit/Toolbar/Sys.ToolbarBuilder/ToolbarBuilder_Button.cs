using static ToSic.Sxc.Edit.Toolbar.Sys.Rules.ToolbarRuleOps;

namespace ToSic.Sxc.Edit.Toolbar.Sys.ToolbarBuilder;

partial record ToolbarBuilder
{

    /// <inheritdoc />
    public IToolbarBuilder Button(
        string name,
        object? target = null,
        NoParamOrder npo = default,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? ui = null,
        object? parameters = null,
        string? operation = null,
        string? context = null
    )
    {
        var pars = PreCleanParams(tweak, defOp: OprNone, operation: operation, ui: ui, parameters: parameters);

        return EntityRule(name, target, pars).Builder;
    }
        
}