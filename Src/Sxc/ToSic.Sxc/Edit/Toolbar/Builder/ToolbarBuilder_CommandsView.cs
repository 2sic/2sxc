using ToSic.Eav;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Web.Url;
using static ToSic.Sxc.Edit.Toolbar.EntityEditInfo;
using static ToSic.Sxc.Edit.Toolbar.ToolbarRuleOps;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial class ToolbarBuilder
    {
        public IToolbarBuilder Layout(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddAdminAction("layout", noParamOrder, ui, parameters, operation, target);


        public IToolbarBuilder Code(
            object target,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        )
        {
            var paramsWithCode = new ObjectToUrl().SerializeWithChild(parameters, (target as string).HasValue() ? "call=" + target : "", "");
            return AddAdminAction("code", noParamOrder, ui, paramsWithCode, operation, target);
        }

        public IToolbarBuilder Fields(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        )
        {
            var pars = PrecleanParams(operation, OprAdd, ui, null, null, parameters, null);
            return EntityRule("fields", target, pars, propsKeep: new[] { KeyContentType }, contentType: target as string).Builder;
        }


        // TODO
        // - custom!

        public IToolbarBuilder Template(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddAdminAction("template", noParamOrder, ui, parameters, operation, target);

        public IToolbarBuilder Query(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddAdminAction("query", noParamOrder, ui, parameters, operation, target);

        public IToolbarBuilder View(
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null
        ) => AddAdminAction("view", noParamOrder, ui, parameters, operation, target);

    }
}
