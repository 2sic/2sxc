using System;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Web.Url;
using static ToSic.Eav.Parameters;
using static ToSic.Sxc.Edit.Toolbar.EntityEditInfo;
using static ToSic.Sxc.Edit.Toolbar.ToolbarRuleOps;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial class ToolbarBuilder
    {
        public IToolbarBuilder Layout(
            object target = null,
            string noParamOrder = Protector,
            Func<ITweakButton, ITweakButton> tweak = default,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddAdminAction("layout", noParamOrder, ui, parameters, operation, target, tweak);


        public IToolbarBuilder Code(
            object target,
            string noParamOrder = Protector,
            Func<ITweakButton, ITweakButton> tweak = default,
            object ui = null,
            object parameters = null,
            string operation = null
        )
        {
            var paramsWithCode = new ObjectToUrl().SerializeWithChild(parameters, (target as string).HasValue() ? "call=" + target : "", "");
            return AddAdminAction("code", noParamOrder, ui, paramsWithCode, operation, target, tweak);
        }

        public IToolbarBuilder Fields(
            object target = null,
            string noParamOrder = Protector,
            Func<ITweakButton, ITweakButton> tweak = default,
            object ui = null,
            object parameters = null,
            string operation = null
        )
        {
            var tweaks = tweak?.Invoke(new TweakButton());
            var pars = PreCleanParams(operation, OprAdd, ui, null, null, parameters, null, tweaks: tweaks);
            return EntityRule("fields", target, pars, propsKeep: new[] { KeyContentType }, contentType: target as string).Builder;
        }


        public IToolbarBuilder Template(
            object target = null,
            string noParamOrder = Protector,
            Func<ITweakButton, ITweakButton> tweak = default,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddAdminAction("template", noParamOrder, ui, parameters, operation, target, tweak);

        public IToolbarBuilder Query(
            object target = null,
            string noParamOrder = Protector,
            Func<ITweakButton, ITweakButton> tweak = default,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddAdminAction("query", noParamOrder, ui, parameters, operation, target, tweak);

        public IToolbarBuilder View(
            object target = null,
            string noParamOrder = Protector,
            Func<ITweakButton, ITweakButton> tweak = default,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddAdminAction("view", noParamOrder, ui, parameters, operation, target, tweak);

    }
}
