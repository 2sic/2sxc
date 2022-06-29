using ToSic.Eav;
using ToSic.Eav.Plumbing;
using static ToSic.Sxc.Edit.Toolbar.EntityEditInfo;

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
            object parameters)
        {
            Eav.Parameters.Protect(noParamOrder, "See docs", methodName);
            var command = new ToolbarRuleForEntity(commandName, target, contentType: contentType,
                ui: ObjToString(ui), parameters: ObjToString(parameters), propsKeep: KeysOfLists);
            return AddInternal(command);

        }

        public IToolbarBuilder Add(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            string contentType = null,
            object ui = null,
            object parameters = null)
        {
            Eav.Parameters.Protect(noParamOrder, "See docs");
            // Special case: Add could be called to "add a button"
            // There is an edge case in the Events app where this was published
            // Must decide if we should keep this or not
            if (target is string strTarget && strTarget.HasValue() && strTarget.Length > 3)
                return AddInternal(strTarget);

            return AddListAction(nameof(Add), "add", target, noParamOrder, contentType, ui, parameters);

            //var addCommand = new ToolbarRuleForEntity("add", target, contentType: contentType,
            //    ui: ObjToString(ui), parameters: ObjToString(parameters), propsToSerialize: KeysOfLists);
            //return AddInternal(addCommand);
        }

        public IToolbarBuilder AddExisting(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            string contentType = null,
            object ui = null,
            object parameters = null
        ) => AddListAction(nameof(AddExisting), "add-existing", target, noParamOrder, contentType, ui, parameters);

        public IToolbarBuilder List(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            //string contentType = null,
            object ui = null,
            object parameters = null
        ) => AddListAction(nameof(List), "list", target, noParamOrder, /*contentType*/null, ui, parameters);


        public IToolbarBuilder MoveDown(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null
        ) => AddListAction(nameof(MoveDown), "movedown", target, noParamOrder, null, ui, parameters);

        public IToolbarBuilder MoveUp(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null
        ) => AddListAction(nameof(MoveUp), "moveup", target, noParamOrder, null, ui, parameters);

        public IToolbarBuilder Remove(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null
        ) => AddListAction(nameof(Remove), "remove", target, noParamOrder, null, ui, parameters);

        public IToolbarBuilder Replace(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null
        ) => AddListAction(nameof(Replace), "replace", target, noParamOrder, null, ui, parameters);
    }
}
