﻿using System.Runtime.CompilerServices;
using ToSic.Sxc.Edit.Toolbar.Sys.Rules;
using static ToSic.Sxc.Edit.Toolbar.Sys.EntityEditInfo;
using static ToSic.Sxc.Edit.Toolbar.Sys.Rules.ToolbarRuleOps;


namespace ToSic.Sxc.Edit.Toolbar.Sys.ToolbarBuilder;

partial record ToolbarBuilder
{
    private class CleanedParams
    {
        public char Operation { get; init; }
        public string? Ui { get; init; }
        public string? Parameters { get; init; }
    }

    private class CleanedParamsWithParts: CleanedParams
    {
        public Dictionary<string, CleanedParams>? Parts;
    }

    private CleanedParamsWithParts PreCleanParams(
        Func<ITweakButton, ITweakButton>? tweak,
        ToolbarRuleOps defOp, 
        NoParamOrder noParamOrder = default,
        string? operation = default, 
        object? ui = default, 
        object? uiMerge = default, 
        string? uiMergePrefix = default, 
        object? parameters = default, 
        object? prefill = default,
        object? filter = default,
        string? fields = default,
        ITweakButton? initialButton = default,
        [CallerMemberName] string? methodName = default)
    {
        // process tweaks, but skip early to reduce calls if null
        var tweaks = tweak == null
            ? null
            : RunTweaksOrErrorIfCombined(tweak: tweak, initial: initialButton, ui: ui, parameters: parameters, prefill: prefill, filter: filter, methodName: methodName);

        var paramsString = tweaks == null && parameters == null
            ? null
            : Utils.PrepareParams(parameters, tweaks);

        var parsWithPrefill = Utils.Prefill2Url.SerializeWithChild(paramsString, prefill, ToolbarConstants.RuleParamPrefixPrefill);
        if (fields != null)
            parsWithPrefill = Utils.Filter2Url.SerializeWithChild(parsWithPrefill, new { fields });

        var namedParts = tweaks?.Named.Any() == true
            ? tweaks.Named
                .ToDictionary(
                    kvp => kvp.Key,
                    CleanedParams (kvp) => PreCleanParams(tweak: kvp.Value, defOp: OprNone)
                )
            : null;

        return new()
        {
            Operation = ToolbarRuleOperation.Pick(operation, defOp, tweaks?.ConditionValue),
            Ui = PrepareUi(ui, uiMerge, uiMergePrefix, tweaks: tweaks?.UiMerge),
            Parameters = Utils.Filter2Url.SerializeWithChild(parsWithPrefill, filter, ToolbarConstants.RuleParamPrefixFilter),
            Parts = namedParts
        };

    }

    private (ToolbarRuleForEntity Rule, IToolbarBuilder Builder) EntityRule(
        string verb, 
        object? target,
        CleanedParams pars,
        string []? propsSkip = null,
        string[]? propsKeep = null,
        string? contentType = null
    )
    {
        TargetCheck(target);
        var command = new ToolbarRuleForEntity(verb, target, pars.Operation,
            ui: pars.Ui, parameters: pars.Parameters,
            contentType: contentType,
            propsKeep: propsKeep, propsSkip: propsSkip,
            decoHelper: Services.ToolbarButtonHelper.Value);
        var builder = this.AddInternal([command], methodName: verb);
        return (command, builder);
    }

    public IToolbarBuilder Delete(
        object? target = null,
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? ui = null,
        object? parameters = null,
        string? operation = null)
    {
        // Set default operation based on what toolbar is used
        var isDefToolbar = FindRule<ToolbarRuleToolbar>()?.IsDefault ?? false;
        var defOp = isDefToolbar ? OprModify : OprAdd;

        var pars = PreCleanParams(tweak, defOp: defOp, operation: operation, ui: ui, uiMerge: "show=true", parameters: parameters);

        return EntityRule("delete", target, pars, 
            propsKeep: [KeyTitle, KeyEntityId, KeyEntityGuid]).Builder;
    }

    public IToolbarBuilder Edit(
        object? target = null,
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? ui = null,
        object? parameters = null,
        object? prefill = null,
        string? operation = null)
    {
        var pars = PreCleanParams(tweak, defOp: OprAdd, operation: operation, ui: ui, parameters: parameters, prefill: prefill);
        return EntityRule("edit", target, pars, propsSkip: [KeyEntityGuid, KeyTitle, KeyPublished]).Builder;
    }

    internal const string BetaEditUiFieldsParamName = "uifields";

    public IToolbarBuilder New(
        object? target = null,
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? ui = null,
        object? parameters = null,
        object? prefill = null,
        string? operation = null)
    {
        var pars = PreCleanParams(tweak, defOp: OprAdd, operation: operation, ui: ui, parameters: parameters, prefill: prefill);

        return EntityRule("new", target, pars,
            propsSkip: [KeyEntityGuid, KeyEntityId, KeyTitle, KeyPublished],
            contentType: target as string).Builder;
    }

    public IToolbarBuilder Publish(
        object? target = null,
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? ui = null,
        object? parameters = null,
        string? operation = null)
    {
        var pars = PreCleanParams(tweak, defOp: OprAdd, operation: operation, ui: ui, parameters: parameters);

        return EntityRule("publish", target, pars,
            propsKeep: [KeyEntityId, KeyPublished, KeyIndex, KeyUseModule]).Builder;
    }


    /// <inheritdoc />
    public IToolbarBuilder Metadata(
        object target,
        string? contentTypes = null,
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? ui = null,
        object? parameters = null,
        object? prefill = null,
        string? operation = null,
        string? context = null)
    {
        var l = Log.Fn<IToolbarBuilder>();
        var pars = PreCleanParams(tweak, defOp: OprAdd, operation: operation, ui: ui, parameters: parameters, prefill: prefill);

        // Note: DO NOT check the target, as here an IAsset is absolutely valid
        // TargetCheck(target);

        var finalTypes = GetMetadataTypeNames(target, contentTypes);
        var realContext = GenerateContext(target, context);

        var mdsToAdd = finalTypes
            .Select(ToolbarRuleBase (type) =>
            {
                var parsForThis = pars.Parts?.TryGetValue(type, out var p) == true
                    ? p
                    : pars;

                return new ToolbarRuleMetadata(
                    target,
                    type,
                    operation: parsForThis.Operation,
                    ui: parsForThis.Ui,
                    parameters: parsForThis.Parameters,
                    context: realContext,
                    decoHelper: Services.ToolbarButtonHelper.Value
                );
            })
            .ToArray();

        // var builder = this as IToolbarBuilder;
        return l.ReturnAsOk(this.AddInternal(mdsToAdd));
    }

    /// <inheritdoc />
    public IToolbarBuilder Copy(
        object? target = null,
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton>? tweak = default,
        string? contentType = null,
        object? ui = null,
        object? parameters = null,
        object? prefill = null,
        string? operation = null,
        string? context = null)
    {
        var pars = PreCleanParams(tweak, defOp: OprAdd, operation: operation, ui: ui, parameters: parameters, prefill: prefill);

        return EntityRule("copy", target, pars, propsKeep: [KeyEntityId, KeyContentType],
            contentType: contentType).Builder;
    }




    public IToolbarBuilder Data(
        object? target = null,
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? filter = null,
        object? ui = null,
        object? parameters = null,
        string? operation = null
    )
    {
        var pars = PreCleanParams(tweak, defOp: OprAdd, operation: operation, ui: ui, parameters: parameters, filter: filter);

        return EntityRule("data", target, pars, propsKeep: [KeyContentType], contentType: target as string)
            .Builder;
    }
        
}