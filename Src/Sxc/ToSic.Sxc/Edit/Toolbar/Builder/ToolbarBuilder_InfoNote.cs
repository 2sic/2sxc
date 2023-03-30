using System;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Web.Url;
using static ToSic.Sxc.Edit.Toolbar.ToolbarRuleOps;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial class ToolbarBuilder
    {

        [PrivateApi("WIP v15.04")]
        public IToolbarBuilder Info(
            string noParamOrder = Eav.Parameters.Protector,
            Func<ITweakButton, ITweakButton> tweak = default,
            string link = default)
            => InfoLikeButton(
                noParamOrder: noParamOrder,
                commandName: "info",
                parametersMerge: link != default ? new { link, } : null,
                tweak: tweak
            );


        private IToolbarBuilder InfoLikeButton(
            string noParamOrder,
            string commandName,
            object parametersMerge,
            Func<ITweakButton, ITweakButton> tweak
        )
        {
            var tweaks = tweak?.Invoke(new TweakButton());
            Eav.Parameters.Protect(noParamOrder, "See docs");
            var paramsWithMessage = new ObjectToUrl().SerializeWithChild(default, parametersMerge);
            var pars = PreCleanParams(operation: default, defOp: OprNone, ui: null, uiMerge: null, uiMergePrefix: null, parameters: paramsWithMessage, prefill: null, tweaks: tweaks);
            return EntityRule(commandName, null, pars).Builder;

        }
    }
}
