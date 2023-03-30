using System;
using System.Runtime.CompilerServices;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial class ToolbarBuilder
    {
        private IToolbarBuilder AddAdminAction(
            string verb,
            string noParamOrder,
            object ui,
            object parameters,
            string operation,
            object target,
            Func<ITweakButton, ITweakButton> tweak,
            [CallerMemberName] string methodName = null
            )
        {
            Eav.Parameters.Protect(noParamOrder, "See docs", methodName);
            var tweaks = RunTweaksOrErrorIfCombined(tweak: tweak, ui: ui, parameters: parameters, methodName: methodName);
            var uiTweaked = PrepareUi(ui, tweaks: tweaks?.UiMerge);
            var paramsTweaked = Utils.PrepareParams(parameters, tweaks);
            TargetCheck(target);
            return AddInternal(new ToolbarRuleCustom(
                verb,
                operation: ToolbarRuleOperation.Pick(operation, ToolbarRuleOps.OprAuto),
                ui: uiTweaked,
                parameters: paramsTweaked,
                operationCode: operation.HasValue() ? null : target as string));
        }
        
        
        public IToolbarBuilder App(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            Func<ITweakButton, ITweakButton> tweak = default,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddAdminAction("app", noParamOrder, ui, parameters, operation, target, tweak);

        public IToolbarBuilder AppImport(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            Func<ITweakButton, ITweakButton> tweak = default,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddAdminAction("app-import", noParamOrder, ui, parameters, operation, target, tweak);
        
        public IToolbarBuilder AppResources(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            Func<ITweakButton, ITweakButton> tweak = default,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddAdminAction("app-resources", noParamOrder, ui, parameters, operation, target, tweak);

        public IToolbarBuilder AppSettings(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            Func<ITweakButton, ITweakButton> tweak = default,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddAdminAction("app-settings", noParamOrder, ui, parameters, operation, target, tweak);

        public IToolbarBuilder Apps(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            Func<ITweakButton, ITweakButton> tweak = default,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddAdminAction("apps", noParamOrder, ui, parameters, operation, target, tweak);

        public IToolbarBuilder System(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            Func<ITweakButton, ITweakButton> tweak = default,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddAdminAction("system", noParamOrder, ui, parameters, operation, target, tweak);


        public IToolbarBuilder Insights(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            Func<ITweakButton, ITweakButton> tweak = default,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddAdminAction("insights", noParamOrder, ui, parameters, operation, target, tweak);

    }
}
