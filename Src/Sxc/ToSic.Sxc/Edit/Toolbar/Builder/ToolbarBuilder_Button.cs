using static ToSic.Sxc.Edit.Toolbar.ToolbarRuleOps;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial class ToolbarBuilder
    {

        /// <inheritdoc />
        public IToolbarBuilder Button(
            string name,
            object target = null,
            string noParamOrder = Eav.Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null,
            string context = null
        )
        {
            Eav.Parameters.Protect(noParamOrder, "See docs");
            var pars = PrecleanParams(operation, OprNone, ui, null, null, parameters, null);

            return EntityRule(name, target, pars).Builder;
        }
        
    }
}
