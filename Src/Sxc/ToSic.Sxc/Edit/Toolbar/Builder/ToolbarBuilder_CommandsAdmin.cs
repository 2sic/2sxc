using System.Runtime.CompilerServices;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial class ToolbarBuilder
    {
        private IToolbarBuilder AddAdminAction(
            string commandName,
            string noParamOrder,
            object ui,
            object parameters,
            string operation,
            object target,
            [CallerMemberName] string methodName = null
            )
        {
            Eav.Parameters.Protect(noParamOrder, "See docs", methodName);
            TargetCheck(target);
            return AddInternal(new ToolbarRuleCustom(
                commandName,
                operation: ToolbarRuleOperation.Pick(operation, ToolbarRuleOps.OprAuto),
                ui: PrepareUi(ui),
                parameters: Utils.Par2Url.Serialize(parameters),
                operationCode: operation.HasValue() ? null : target as string));
        }
        
        
        public IToolbarBuilder App(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddAdminAction("app", noParamOrder, ui, parameters, operation, target);

        public IToolbarBuilder AppImport(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddAdminAction("app-import", noParamOrder, ui, parameters, operation, target);
        
        public IToolbarBuilder AppResources(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddAdminAction("app-resources", noParamOrder, ui, parameters, operation, target);

        public IToolbarBuilder AppSettings(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddAdminAction("app-settings", noParamOrder, ui, parameters, operation, target);

        public IToolbarBuilder Apps(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddAdminAction("apps", noParamOrder, ui, parameters, operation, target);

        public IToolbarBuilder System(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddAdminAction("system", noParamOrder, ui, parameters, operation, target);


        public IToolbarBuilder Insights(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddAdminAction("insights", noParamOrder, ui, parameters, operation, target);

    }
}
