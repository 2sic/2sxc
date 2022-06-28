using ToSic.Eav;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial class ToolbarBuilder
    {
        // TODO: GENERIC METHOD TO PRESERVE TEMPLATE INFOS
        public IToolbarBuilder Layout(
            object target = null,
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddAdminAction("layout", noParamOrder, ui, parameters, operation);

        public IToolbarBuilder TemplateEdit(
            object target = null,
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddAdminAction("template-develop", noParamOrder, ui, parameters, operation);

        public IToolbarBuilder QueryEdit(
            object target = null,
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddAdminAction("template-query", noParamOrder, ui, parameters, operation);

        public IToolbarBuilder ViewEdit(
            object target = null,
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddAdminAction("template-settings", noParamOrder, ui, parameters, operation);

    }
}
