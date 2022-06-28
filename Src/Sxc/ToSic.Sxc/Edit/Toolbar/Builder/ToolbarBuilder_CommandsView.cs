using ToSic.Eav;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Web.Url;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial class ToolbarBuilder
    {
        // TODO: GENERIC METHOD TO PRESERVE TEMPLATE INFOS
        public IToolbarBuilder Layout(
            object target,
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddAdminAction("layout", noParamOrder, ui, parameters, operation);


        public IToolbarBuilder Code(
            object target,
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        )
        {
            var paramsWithCode = new ObjectToUrl().SerializeWithChild(parameters, (target as string).HasValue() ? "call=" + target : "", "");
            return AddAdminAction("code", noParamOrder, ui, paramsWithCode, operation ?? target as string);
        }

        public IToolbarBuilder ContentType(
            object target,
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddAdminAction("contenttype", noParamOrder, ui, parameters, operation);
        
        
        // TODO
        // - contenttype - make work with entity inside...
        // - 

        public IToolbarBuilder TemplateEdit(
            object target = null,
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddAdminAction("template-develop", noParamOrder, ui, parameters, operation ?? target as string);

        public IToolbarBuilder QueryEdit(
            object target = null,
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddAdminAction("template-query", noParamOrder, ui, parameters, operation ?? target as string);

        public IToolbarBuilder ViewEdit(
            object target = null,
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddAdminAction("template-settings", noParamOrder, ui, parameters, operation ?? target as string);

    }
}
