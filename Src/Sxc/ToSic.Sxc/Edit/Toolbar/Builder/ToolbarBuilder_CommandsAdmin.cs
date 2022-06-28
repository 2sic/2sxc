using System.Runtime.CompilerServices;
using ToSic.Eav;

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
            [CallerMemberName] string methodName = null
            )
        {
            Parameters.Protect(noParamOrder, "See docs", methodName);
            var command = new ToolbarRuleCustom(commandName, 
                operation: ToolbarRuleOps.Pick(operation, ToolbarRuleOperations.BtnAddAuto),
                ui: ObjToString(ui), parameters: ObjToString(parameters));
            return AddInternal(command);
        }
        
        
        public IToolbarBuilder App(
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddAdminAction("app", noParamOrder, ui, parameters, operation);

        public IToolbarBuilder AppImport(
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddAdminAction("app-import", noParamOrder, ui, parameters, operation);
        
        public IToolbarBuilder AppResources(
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddAdminAction("app-resources", noParamOrder, ui, parameters, operation);

        public IToolbarBuilder AppSettings(
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddAdminAction("app-settings", noParamOrder, ui, parameters, operation);

        public IToolbarBuilder Apps(
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddAdminAction("apps", noParamOrder, ui, parameters, operation);

        public IToolbarBuilder System(
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddAdminAction("system", noParamOrder, ui, parameters, operation);


        public IToolbarBuilder Insights(
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddAdminAction("insights", noParamOrder, ui, parameters, operation);

        

    }
}
