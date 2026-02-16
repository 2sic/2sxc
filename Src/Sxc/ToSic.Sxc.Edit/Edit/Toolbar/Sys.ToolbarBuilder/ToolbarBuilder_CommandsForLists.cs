using System.Runtime.CompilerServices;
using ToSic.Sxc.Edit.Toolbar.Sys.Rules;
using static ToSic.Sxc.Edit.Toolbar.Sys.EntityEditInfo;
using static ToSic.Sxc.Edit.Toolbar.Sys.Rules.ToolbarRuleOps;

namespace ToSic.Sxc.Edit.Toolbar.Sys.ToolbarBuilder;

partial record ToolbarBuilder
{
    private IToolbarBuilder AddListAction(
        string commandName,
        object? target,
        NoParamOrder npo,
        string? contentType,
        object? ui,
        object? parameters,
        string? operation,
        Func<ITweakButton, ITweakButton>? tweak,
        [CallerMemberName] string? methodName = default)
    {
        TargetCheck(target);
        var pars = PreCleanParams(tweak, defOp: OprAuto, operation: operation, ui: ui, parameters: parameters, methodName: methodName);
        var command = new ToolbarRuleForEntity(commandName, target, 
            contentType: contentType,
            ui: pars.Ui,
            parameters: pars.Parameters,
            propsKeep: KeysOfLists, 
            operation: pars.Operation);
        return this.AddInternal([command], methodName: methodName);

    }


    public IToolbarBuilder Add(
        object? target = null,
        NoParamOrder npo = default,
        string? contentType = null,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? ui = null,
        object? parameters = null,
        string? operation = null
    ) => AddListAction(ActionNames.Add, target, npo, contentType, ui, parameters, operation, tweak);

    public IToolbarBuilder AddExisting(
        object? target = null,
        NoParamOrder npo = default,
        string? contentType = null,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? ui = null,
        object? parameters = null,
        string? operation = null
    ) => AddListAction(ActionNames.AddExisting, target, npo, contentType, ui, parameters, operation, tweak);

    public IToolbarBuilder List(
        object? target = null,
        NoParamOrder npo = default,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? ui = null,
        object? parameters = null,
        string? operation = null
    ) => AddListAction(ActionNames.List, target, npo, null, ui, parameters, operation, tweak);


    public IToolbarBuilder MoveDown(
        object? target = null,
        NoParamOrder npo = default,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? ui = null,
        object? parameters = null,
        string? operation = null
    ) => AddListAction(ActionNames.MoveDown, target, npo, null, ui, parameters, operation, tweak);

    public IToolbarBuilder MoveUp(
        object? target = null,
        NoParamOrder npo = default,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? ui = null,
        object? parameters = null,
        string? operation = null
    ) => AddListAction(ActionNames.MoveUp, target, npo, null, ui, parameters, operation, tweak);

    public IToolbarBuilder Remove(
        object? target = null,
        NoParamOrder npo = default,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? ui = null,
        object? parameters = null,
        string? operation = null
    ) => AddListAction(ActionNames.Remove, target, npo, null, ui, parameters, operation, tweak);

    public IToolbarBuilder Replace(
        object? target = null,
        NoParamOrder npo = default,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? ui = null,
        object? parameters = null,
        string? operation = null
    ) => AddListAction(ActionNames.Replace, target, npo, null, ui, parameters, operation, tweak);
}