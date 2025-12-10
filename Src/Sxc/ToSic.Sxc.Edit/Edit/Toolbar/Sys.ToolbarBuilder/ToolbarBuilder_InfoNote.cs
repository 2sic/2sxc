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
        verb: "info",
        paramsMergeInTweak: link != default ? new { link, target } : null,
        tweak: tweak);


    private IToolbarBuilder InfoLikeButton(
        NoParamOrder npo,
        string verb,
        object? paramsMergeInTweak,
        Func<ITweakButton, ITweakButton>? tweak
    )
    {
        tweak ??= TweakButton.TweakButton.NoOp; 
        var initial = paramsMergeInTweak == null
            ? null
            : new TweakButton.TweakButton().Parameters(paramsMergeInTweak);
        var pars = PreCleanParams(tweak, defOp: OprNone, initialButton: initial);
        return EntityRule(verb, null, pars).Builder;
    }
}