using static ToSic.Sxc.Edit.Toolbar.ToolbarRuleOps;

namespace ToSic.Sxc.Edit.Toolbar.Internal;

partial class ToolbarBuilder
{
    public IToolbarBuilder Info(
        NoParamOrder noParamOrder = default,
        string link = default,
        string target = default,
        Func<ITweakButton, ITweakButton> tweak = default
    ) => InfoLikeButton(noParamOrder: noParamOrder, verb: "info", paramsMergeInTweak: link != default ? new { link, target } : null, tweak: tweak);


    private IToolbarBuilder InfoLikeButton(
        NoParamOrder noParamOrder,
        string verb,
        object paramsMergeInTweak,
        Func<ITweakButton, ITweakButton> tweak
    )
    {
        tweak ??= TweakButton.NoOp; 
        var initial = paramsMergeInTweak == null ? null : new TweakButton().Parameters(paramsMergeInTweak);
        var pars = PreCleanParams(tweak, defOp: OprNone, initialButton: initial);
        return EntityRule(verb, null, pars).Builder;
    }
}