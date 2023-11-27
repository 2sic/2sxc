using System;
using System.Runtime.CompilerServices;
using ToSic.Lib.Coding;
using ToSic.Lib.Documentation;
using static ToSic.Sxc.Edit.Toolbar.ToolbarRuleOps;

namespace ToSic.Sxc.Edit.Toolbar;

public partial class ToolbarBuilder
{

    [PrivateApi("WIP v15.04")]
    public IToolbarBuilder Info(
        NoParamOrder noParamOrder = default,
        string link = default,
        Func<ITweakButton, ITweakButton> tweak = default
    ) => InfoLikeButton(noParamOrder: noParamOrder, verb: "info", paramsMergeInTweak: link != default ? new { link, } : null, tweak: tweak);


    private IToolbarBuilder InfoLikeButton(
        NoParamOrder noParamOrder,
        string verb,
        object paramsMergeInTweak,
        Func<ITweakButton, ITweakButton> tweak,
        [CallerMemberName] string methodName = null
    )
    {
        //Eav.Parameters.Protect(noParamOrder, "See docs", methodName: methodName);
        tweak ??= TweakButton.NoOp; 
        var initial = paramsMergeInTweak == null ? null : new TweakButton().Parameters(paramsMergeInTweak);
        var pars = PreCleanParams(tweak, defOp: OprNone, initialButton: initial);
        return EntityRule(verb, null, pars).Builder;
    }
}