using static ToSic.Sxc.Edit.Toolbar.EntityEditInfo;
using static ToSic.Sxc.Edit.Toolbar.ToolbarRuleOps;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial class ToolbarBuilder
    {
        private IToolbarBuilder AddListAction(
            string methodName, 
            string commandName,
            object target,
            string noParamOrder,
            string contentType,
            object ui,
            object parameters,
            string operation)
        {
            Eav.Parameters.Protect(noParamOrder, "See docs", methodName);
            TargetCheck(target);
            var pars = PrecleanParams(operation, OprAuto, ui, null, null, parameters, null);
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
            string noParamOrder = Eav.Parameters.Protector,
            string contentType = null,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddListAction(nameof(Add), "add", target, noParamOrder, contentType, ui, parameters, operation);

        public IToolbarBuilder AddExisting(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            string contentType = null,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddListAction(nameof(AddExisting), "add-existing", target, noParamOrder, contentType, ui, parameters, operation);

        public IToolbarBuilder List(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddListAction(nameof(List), "list", target, noParamOrder, null, ui, parameters, operation);


        public IToolbarBuilder MoveDown(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddListAction(nameof(MoveDown), "movedown", target, noParamOrder, null, ui, parameters, operation);

        public IToolbarBuilder MoveUp(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddListAction(nameof(MoveUp), "moveup", target, noParamOrder, null, ui, parameters, operation);

        public IToolbarBuilder Remove(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddListAction(nameof(Remove), "remove", target, noParamOrder, null, ui, parameters, operation);

        public IToolbarBuilder Replace(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddListAction(nameof(Replace), "replace", target, noParamOrder, null, ui, parameters, operation);
    }
}
