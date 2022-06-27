using ToSic.Eav;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial class ToolbarBuilder
    {
        private IToolbarBuilder AddAdminAction(
            string methodName, 
            string commandName,
            string noParamOrder,
            bool? show,
            object ui,
            object parameters)
        {
            Parameters.ProtectAgainstMissingParameterNames(noParamOrder, methodName, "See docs");
            var command = new ToolbarRuleCustom(commandName, operation: OperationShow(show),
                ui: ObjToString(ui), parameters: ObjToString(parameters));
            return AddInternal(command);

        }
        
        
        public IToolbarBuilder App(
            string noParamOrder = Parameters.Protector,
            bool? show = null,
            object ui = null,
            object parameters = null
        ) => AddAdminAction(nameof(App), "app", noParamOrder, show, ui, parameters);

        public IToolbarBuilder AppImport(
            string noParamOrder = Parameters.Protector,
            bool? show = null,
            object ui = null,
            object parameters = null
        ) => AddAdminAction(nameof(AppImport), "app-import", noParamOrder, show, ui, parameters);
        
        public IToolbarBuilder AppResources(
            string noParamOrder = Parameters.Protector,
            bool? show = null,
            object ui = null,
            object parameters = null
        ) => AddAdminAction(nameof(AppResources), "app-resources", noParamOrder, show, ui, parameters);

        public IToolbarBuilder AppSettings(
            string noParamOrder = Parameters.Protector,
            bool? show = null,
            object ui = null,
            object parameters = null
        ) => AddAdminAction(nameof(AppSettings), "app-settings", noParamOrder, show, ui, parameters);

        public IToolbarBuilder Apps(
            string noParamOrder = Parameters.Protector,
            bool? show = null,
            object ui = null,
            object parameters = null
        ) => AddAdminAction(nameof(Apps), "apps", noParamOrder, show, ui, parameters);

        public IToolbarBuilder System(
            string noParamOrder = Parameters.Protector,
            bool? show = null,
            object ui = null,
            object parameters = null
        ) => AddAdminAction(nameof(System), "system", noParamOrder, show, ui, parameters);


        public IToolbarBuilder Insights(
            string noParamOrder = Parameters.Protector,
            bool? show = null,
            object ui = null,
            object parameters = null
        ) => AddAdminAction(nameof(Insights), "insights", noParamOrder, show, ui, parameters);

        

    }
}
