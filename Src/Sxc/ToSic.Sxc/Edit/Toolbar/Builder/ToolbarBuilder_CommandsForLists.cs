using System;
using System.Runtime.CompilerServices;
using static ToSic.Eav.Parameters;
using static ToSic.Sxc.Edit.Toolbar.EntityEditInfo;
using static ToSic.Sxc.Edit.Toolbar.ToolbarRuleOps;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial class ToolbarBuilder
    {
        private IToolbarBuilder AddListAction(
            string commandName,
            object target,
            string noParamOrder,
            string contentType,
            object ui,
            object parameters,
            string operation,
            Func<ITweakButton, ITweakButton> tweak,
            [CallerMemberName] string methodName = default)
        {
            Protect(noParamOrder, "See docs", methodName);
            var tweaks = tweak?.Invoke(new TweakButton());
            TargetCheck(target);
            var pars = PreCleanParams(operation, OprAuto, ui, null, null, parameters, null, tweaks: tweaks);
            var command = new ToolbarRuleForEntity(commandName, target, 
                contentType: contentType,
                ui: pars.Ui,
                parameters: pars.Parameters,
                propsKeep: KeysOfLists, 
                operation: pars.Operation);
            return AddInternal(command);

        }

        public IToolbarBuilder Add(
            object target = null,
            string noParamOrder = Protector,
            Func<ITweakButton, ITweakButton> tweak = default,
            string contentType = null,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddListAction("add", target, noParamOrder, contentType, ui, parameters, operation, tweak);

        public IToolbarBuilder AddExisting(
            object target = null,
            string noParamOrder = Protector,
            Func<ITweakButton, ITweakButton> tweak = default,
            string contentType = null,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddListAction("add-existing", target, noParamOrder, contentType, ui, parameters, operation, tweak);

        public IToolbarBuilder List(
            object target = null,
            string noParamOrder = Protector,
            Func<ITweakButton, ITweakButton> tweak = default,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddListAction("list", target, noParamOrder, null, ui, parameters, operation, tweak);


        public IToolbarBuilder MoveDown(
            object target = null,
            string noParamOrder = Protector,
            Func<ITweakButton, ITweakButton> tweak = default,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddListAction("movedown", target, noParamOrder, null, ui, parameters, operation, tweak);

        public IToolbarBuilder MoveUp(
            object target = null,
            string noParamOrder = Protector,
            Func<ITweakButton, ITweakButton> tweak = default,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddListAction("moveup", target, noParamOrder, null, ui, parameters, operation, tweak);

        public IToolbarBuilder Remove(
            object target = null,
            string noParamOrder = Protector,
            Func<ITweakButton, ITweakButton> tweak = default,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddListAction("remove", target, noParamOrder, null, ui, parameters, operation, tweak);

        public IToolbarBuilder Replace(
            object target = null,
            string noParamOrder = Protector,
            Func<ITweakButton, ITweakButton> tweak = default,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddListAction("replace", target, noParamOrder, null, ui, parameters, operation, tweak);
    }
}
