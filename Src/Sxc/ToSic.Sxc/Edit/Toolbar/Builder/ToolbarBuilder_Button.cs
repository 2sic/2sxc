using ToSic.Eav;
using static ToSic.Sxc.Edit.Toolbar.ToolbarRuleOperations;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial class ToolbarBuilder
    {

        /// <inheritdoc />
        public IToolbarBuilder Button(
            string name,
            object target = null,
            string noParamOrder = Parameters.Protector,
            object ui = null,
            object parameters = null,
            string operation = null,
            string context = null
        )
        {
            Parameters.Protect(noParamOrder, "See docs");
            var pars = PrecleanParams(operation, OprNone, ui, null, null, parameters, null);

            return EntityRule(name, target, pars).Builder;
        }
        
    }
}
