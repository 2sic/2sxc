using ToSic.Eav;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial class ToolbarBuilder
    {
        private IToolbarBuilder AddAdminAction(
            string methodName, 
            string commandName,
            string noParamOrder,
            object ui,
            object parameters,
            string operation)
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
        ) => AddAdminAction(nameof(App), "app", noParamOrder, ui, parameters, operation);

        public IToolbarBuilder AppImport(
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddAdminAction(nameof(AppImport), "app-import", noParamOrder, ui, parameters, operation);
        
        public IToolbarBuilder AppResources(
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddAdminAction(nameof(AppResources), "app-resources", noParamOrder, ui, parameters, operation);

        public IToolbarBuilder AppSettings(
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddAdminAction(nameof(AppSettings), "app-settings", noParamOrder, ui, parameters, operation);

        public IToolbarBuilder Apps(
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddAdminAction(nameof(Apps), "apps", noParamOrder, ui, parameters, operation);

        public IToolbarBuilder System(
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddAdminAction(nameof(System), "system", noParamOrder, ui, parameters, operation);


        public IToolbarBuilder Insights(
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddAdminAction(nameof(Insights), "insights", noParamOrder, ui, parameters, operation);

        

    }
}
