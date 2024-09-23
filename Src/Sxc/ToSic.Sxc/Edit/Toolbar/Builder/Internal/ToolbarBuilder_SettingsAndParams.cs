using ToSic.Eav.Apps.Assets;

namespace ToSic.Sxc.Edit.Toolbar.Internal;

partial class ToolbarBuilder
{
    public IToolbarBuilder Settings(
        NoParamOrder noParamOrder = default,
        string show = default,
        string hover = default,
        string follow = default,
        string classes = default,
        string autoAddMore = default,
        object ui = default,
        object parameters = default)
        => this.AddInternal(new ToolbarRuleSettings(show: show, hover: hover, follow: follow, classes: classes, autoAddMore: autoAddMore,
            ui: PrepareUi(ui), parameters: Utils.Par2Url.Serialize(parameters)));


    public IToolbarBuilder Parameters(
        object target = default,
        NoParamOrder noParamOrder = default,
        Func<ITweakButton, ITweakButton> tweak = default,
        object ui = default,
        object parameters = default,
        object prefill = default,
        string context = default
    )
    {
        TargetCheck(target);
        var clone = new ToolbarBuilder(this);

        // see if we already have a params rule, if yes remove to then later clone and add again
        var previous = clone.FindRule<ToolbarRuleForParams>();
        if (previous != null)
            clone.Rules.Remove(previous);

        // detect if the first parameter (target) is a parameters object
        (target, parameters) = FixTargetIsParameters(target, parameters);

        // Use new or previous target
        target ??= previous?.Target;

        // Must create a new one, to not change the original which is still in the original object
        var uiWithPrevious = PrepareUi(previous?.Ui, ui);
        var partsWithPrevious = Utils.Par2Url.SerializeWithChild(previous?.Parameters, parameters);
        var parts = PreCleanParams(tweak, defOp: ToolbarRuleOps.OprNone, ui: uiWithPrevious, parameters: partsWithPrevious, prefill: prefill);

        //var parsWithPrefill = Utils.Prefill2Url.SerializeWithChild(partsWithPrevious, prefill, PrefixPrefill);

        var newParamsRule = new ToolbarRuleForParams(target,
            parts.Ui,
            parts.Parameters,
            GenerateContext(target, context) ?? previous?.Context,
            Services.ToolbarButtonHelper.Value);

        clone.Rules.Add(newParamsRule);

        return clone;
    }

    private void TargetCheck(object target)
    {
        if (target is IAsset)
            throw new("Got a 'target' parameter which seems to be an adam-file. " +
                      "This is not allowed. " +
                      "Were you trying to target the .Metadata of this file? if so, add .Metadata to the target object.");

    }

    private static (object target, object parameters) FixTargetIsParameters(object target, object parameters)
    {
        // No target, or parameters supplied
        if (parameters != null || target == null) return (target, parameters);

        // Basically only keep the target as is, if it's a known target
        if (target is IEntity or ICanBeEntity or IEnumerable<IEntity> or IEnumerable<ICanBeEntity>)
            return (target, null);

        return (null, target);
    }
}