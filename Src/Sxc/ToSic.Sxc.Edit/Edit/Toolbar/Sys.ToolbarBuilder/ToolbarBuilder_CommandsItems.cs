using System.Runtime.CompilerServices;
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
        /// <summary>
        /// If the tweaks have any named sub-tweaks, this will contain the pre-cleaned parameters for each of those sub-tweaks.
        /// Used in Metadata which could produce multiple buttons, each with its own tweaks.
        /// </summary>
        public Dictionary<string, CleanedParams>? Parts;
    }

    private CleanedParamsWithParts PreCleanParams(
        Func<ITweakButton, ITweakButton>? tweak,
        ToolbarRuleOps defOp, 
        NoParamOrder npo = default,
        string? operation = default, 
        object? ui = default, 
        object? uiMerge = default, 
        string? uiMergePrefix = default, 
        object? parameters = default, 
        object? prefill = default,
        object? filter = default,
        string? fields = default,
        ITweakButton? tweakMixin = default, // this is usually null, but allows for more specs to be set to mix with the tweak
        [CallerMemberName] string? methodName = default)
    {
        // Process tweaks #1, but skip early to reduce calls if null
        var tweaks = tweak == null && tweakMixin == null
            ? null
            : RunTweaksOrErrorIfCombined(tweak: tweak, tweakMixin: tweakMixin, ui: ui, parameters: parameters, prefill: prefill, filter: filter, methodName: methodName);

        // Process tweaks #2
        var paramsString = tweaks == null && parameters == null
            ? null
            : Utils.PrepareParams(parameters, tweaks);

        // Combine parameters with prefill and fields
        var parsWithPrefill = Utils.Prefill2Url.SerializeWithChild(paramsString, prefill, ToolbarConstants.RuleParamPrefixPrefill);
        if (fields != null)
            parsWithPrefill = Utils.Filter2Url.SerializeWithChild(parsWithPrefill, new { fields });

        // Check if the tweaks have any named sub-tweaks
        // Basically this means that we have tweaks which only apply to specific targets - used in Metadata
        // So in case metadata has many buttons (for different metadata types) they could have different tweaks configuring each
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

    private (ToolbarRuleForEntity Rule, IToolbarBuilder Builder) AddEntityRule(
        string verb, 
        object? target,
        CleanedParams pars,
        string[]? propsSkip = null,
        string[]? propsKeep = null,
        string? contentType = null
    )
    {
        TargetCheck(target);
        var command = new ToolbarRuleForEntity(
            commandName: verb,
            decoHelper: Services.ToolbarButtonHelper.Value,
            target: target,
            operation: pars.Operation,
            ui: pars.Ui,
            parameters: pars.Parameters,
            contentType: contentType,
            propsKeep: propsKeep,
            propsSkip: propsSkip
        );
        var builder = this.AddInternal([command], methodName: verb);
        return (command, builder);
    }

    public IToolbarBuilder Delete(
        object? target = null,
        NoParamOrder npo = default,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? ui = null,
        object? parameters = null,
        string? operation = null)
    {
        // Set default operation based on what toolbar is used
        var isDefToolbar = FindRule<ToolbarRuleToolbar>()?.IsDefault ?? false;
        var defOp = isDefToolbar ? OprModify : OprAdd;

        var pars = PreCleanParams(tweak, defOp: defOp, operation: operation, ui: ui, uiMerge: "show=true", parameters: parameters);

        return AddEntityRule(ActionNames.Delete, target, pars, 
            propsKeep: [KeyTitle, KeyEntityId, KeyEntityGuid]).Builder;
    }

    public IToolbarBuilder Edit(
        object? target = null,
        NoParamOrder npo = default,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? ui = null,
        object? parameters = null,
        object? prefill = null,
        string? operation = null)
    {
        var pars = PreCleanParams(tweak, defOp: OprAdd, operation: operation, ui: ui, parameters: parameters, prefill: prefill);
        return AddEntityRule(ActionNames.Edit, target, pars, propsSkip: [KeyEntityGuid, KeyTitle, KeyPublished]).Builder;
    }

    public IToolbarBuilder New(
        object? target = null,
        NoParamOrder npo = default,
        object? contentType = null,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? ui = null,
        object? parameters = null,
        object? prefill = null,
        string? operation = null)
    {
        var pars = PreCleanParams(tweak, defOp: OprAdd, operation: operation, ui: ui, parameters: parameters, prefill: prefill);

        var contentTypeName =
            contentType as string                           // contentType as string
            ?? (contentType as IContentType)?.NameId        // contentType as IContentType
            ?? (contentType as Type)?.Name                  // contentType as .net Type
            ?? target as string;                            // fallback and oldest implementation, where the contentType was passed as target

        return AddEntityRule(verb: ActionNames.New,
            target: target,
            pars: pars,
            propsSkip: [KeyEntityGuid, KeyEntityId, KeyTitle, KeyPublished],
            contentType: contentTypeName
        ).Builder;
    }



    public IToolbarBuilder Publish(
        object? target = null,
        NoParamOrder npo = default,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? ui = null,
        object? parameters = null,
        string? operation = null)
    {
        var pars = PreCleanParams(tweak, defOp: OprAdd, operation: operation, ui: ui, parameters: parameters);

        return AddEntityRule(ActionNames.Publish, target, pars,
            propsKeep: [KeyEntityId, KeyPublished, KeyIndex, KeyUseModule]).Builder;
    }


    /// <inheritdoc />
    public IToolbarBuilder Metadata(
        object target,
        string? contentTypes = null,
        NoParamOrder npo = default,
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
                var partsForThis = pars.Parts?.TryGetValue(type, out var p) == true
                    ? p
                    : pars;

                return new ToolbarRuleMetadata(
                    target: target,
                    typeName: type,
                    operation: partsForThis.Operation,
                    decoHelper: Services.ToolbarButtonHelper.Value,
                    ui: partsForThis.Ui,
                    parameters: partsForThis.Parameters,
                    context: realContext
                );
            })
            .ToArray();

        return l.ReturnAsOk(this.AddInternal(mdsToAdd));
    }

    /// <inheritdoc />
    public IToolbarBuilder Copy(
        object? target = null,
        NoParamOrder npo = default,
        Func<ITweakButton, ITweakButton>? tweak = default,
        string? contentType = null,
        object? ui = null,
        object? parameters = null,
        object? prefill = null,
        string? operation = null,
        string? context = null)
    {
        var pars = PreCleanParams(tweak, defOp: OprAdd, operation: operation, ui: ui, parameters: parameters, prefill: prefill);

        return AddEntityRule(ActionNames.Copy, target, pars, propsKeep: [KeyEntityId, KeyContentType],
            contentType: contentType).Builder;
    }


    public IToolbarBuilder Data(
        object? target = null,
        NoParamOrder npo = default,
        Func<ITweakButton, ITweakButton>? tweak = default,
        object? filter = null,
        object? ui = null,
        object? parameters = null,
        string? operation = null
    )
    {
        var pars = PreCleanParams(tweak, defOp: OprAdd, operation: operation, ui: ui, parameters: parameters, filter: filter);

        return AddEntityRule(ActionNames.Data, target, pars, propsKeep: [KeyContentType], contentType: target as string)
            .Builder;
    }
        
}