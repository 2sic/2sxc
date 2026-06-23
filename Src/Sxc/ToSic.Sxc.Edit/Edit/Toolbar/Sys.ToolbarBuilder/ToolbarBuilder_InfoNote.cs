using static ToSic.Sxc.Edit.Toolbar.Sys.Rules.ToolbarRuleOps;

namespace ToSic.Sxc.Edit.Toolbar.Sys.ToolbarBuilder;

partial record ToolbarBuilder
{
    public IToolbarBuilder Info(
        NoParamOrder npo = default,
        string? link = default,
        string? target = default,
        Func<ITweakButton, ITweakButton>? tweak = default
    ) => InfoLikeButton(npo: npo,
        verb: ActionNames.Info,
        paramsToMergeIntoTweak: link != default ? new { link, target } : null,
        tweak: tweak);

    private IToolbarBuilder InfoLikeButton(
        NoParamOrder npo,
        string verb,
        object? paramsToMergeIntoTweak,
        Func<ITweakButton, ITweakButton>? tweak
    )
    {
        //tweak ??= TweakButton.TweakButton.NoOp; 
        var tweakMixin = paramsToMergeIntoTweak == null
            ? null
            : new TweakButton.TweakButton().Parameters(paramsToMergeIntoTweak);
        var pars = PreCleanParams(tweak, defOp: OprNone, tweakMixin: tweakMixin);
        return AddEntityRule(verb, null, pars).Builder;
    }
}