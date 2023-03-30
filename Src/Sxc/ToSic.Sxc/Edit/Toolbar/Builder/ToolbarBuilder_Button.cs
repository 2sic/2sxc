using System;
using static ToSic.Sxc.Edit.Toolbar.ToolbarRuleOps;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial class ToolbarBuilder
    {

        /// <inheritdoc />
        public IToolbarBuilder Button(
            string name,
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            Func<ITweakButton, ITweakButton> tweak = default,
            object ui = null,
            object parameters = null,
            string operation = null,
            string context = null
        )
        {
            Eav.Parameters.Protect(noParamOrder, "See docs");
            var tweaks = tweak?.Invoke(new TweakButton());
            var pars = PreCleanParams(operation, OprNone, ui, null, null, parameters, null, tweaks);

            return EntityRule(name, target, pars).Builder;
        }
        
    }
}
