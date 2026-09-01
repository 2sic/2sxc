using System.Runtime.CompilerServices;
using ToSic.Sxc.Edit.Toolbar.Sys.Rules;
using static ToSic.Sxc.Edit.Toolbar.Sys.EntityEditInfo;
using static ToSic.Sxc.Edit.Toolbar.Sys.Rules.ToolbarRuleOps;

namespace ToSic.Sxc.Edit.Toolbar.Sys.ToolbarBuilder;

partial record ToolbarBuilder
{
    private IToolbarBuilder AddListRule(
        string commandName,
        object? target,
        NoParamOrder npo,
        string? contentType,
        object? ui,
        object? parameters,
        string? operation,
        Func<ITweakButton, ITweakButton>? tweak,
        ITweakButton? tweakMixin = null,
        [CallerMemberName] string? methodName = default)
    {
        TargetCheck(target);
        var pars = PreCleanParams(tweak, tweakMixin: tweakMixin, defOp: OprAuto, operation: operation, ui: ui, parameters: parameters, methodName: methodName);
        var command = new ToolbarRuleForEntity(
            commandName: commandName,
            decoHelper: Services.ToolbarButtonHelper.Value,
            target: target,
            contentType: contentType,
            ui: pars.Ui,
            parameters: pars.Parameters,
            propsKeep: KeysOfLists,
            operation: pars.Operation
        );
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
    ) => AddListRule(commandName: ActionNames.Add, target: target, npo: npo, contentType: contentType, ui: ui, parameters: parameters, operation: operation, tweak: tweak);

    public IToolbarBuilder AddExisting(
        object? target = null,
        NoParamOrder npo = default,
        string? contentType = null,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? ui = null,
        object? parameters = null,
        string? operation = null
    )
    {
        // if we have a content-type, we must mix it in with the tweak...
        var tweakMixin = contentType == null
            ? null
            : new TweakButton.TweakButton().Parameters("contentType", contentType);
        return AddListRule(commandName: ActionNames.AddExisting, target: target, npo: npo, contentType: contentType,
            ui: ui, parameters: parameters, operation: operation, tweak: tweak, tweakMixin: tweakMixin);
    }

    public IToolbarBuilder List(
        object? target = null,
        NoParamOrder npo = default,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? ui = null,
        object? parameters = null,
        string? operation = null
    ) => AddListRule(commandName: ActionNames.List, target: target, npo: npo, contentType: null, ui: ui, parameters: parameters, operation: operation, tweak: tweak);


    public IToolbarBuilder MoveDown(
        object? target = null,
        NoParamOrder npo = default,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? ui = null,
        object? parameters = null,
        string? operation = null
    ) => AddListRule(commandName: ActionNames.MoveDown, target: target, npo: npo, contentType: null, ui: ui, parameters: parameters, operation: operation, tweak: tweak);

    public IToolbarBuilder MoveUp(
        object? target = null,
        NoParamOrder npo = default,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? ui = null,
        object? parameters = null,
        string? operation = null
    ) => AddListRule(commandName: ActionNames.MoveUp, target: target, npo: npo, contentType: null, ui: ui, parameters: parameters, operation: operation, tweak: tweak);

    public IToolbarBuilder Remove(
        object? target = null,
        NoParamOrder npo = default,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? ui = null,
        object? parameters = null,
        string? operation = null
    ) => AddListRule(commandName: ActionNames.Remove, target: target, npo: npo, contentType: null, ui: ui, parameters: parameters, operation: operation, tweak: tweak);

    public IToolbarBuilder Replace(
        object? target = null,
        NoParamOrder npo = default,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? ui = null,
        object? parameters = null,
        string? operation = null
    ) => AddListRule(commandName: ActionNames.Replace, target: target, npo: npo, contentType: null, ui: ui, parameters: parameters, operation: operation, tweak: tweak);
}