using ToSic.Eav;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial class ToolbarBuilder
    {
        // TODO: GENERIC METHOD TO PRESERVE TEMPLATE INFOS

        public IToolbarBuilder TemplateEdit(
            object target = null,
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null
        ) => AddAdminAction(nameof(TemplateEdit), "template-develop", noParamOrder, null, ui, parameters);

        public IToolbarBuilder QueryEdit(
            object target = null,
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null
        ) => AddAdminAction(nameof(QueryEdit), "template-query", noParamOrder, null, ui, parameters);

        public IToolbarBuilder ViewEdit(
            object target = null,
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null
        ) => AddAdminAction(nameof(ViewEdit), "template-settings", noParamOrder, null, ui, parameters);

    }
}
