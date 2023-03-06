using ToSic.Lib.Documentation;
using ToSic.Sxc.Web.Url;
using static ToSic.Sxc.Edit.Toolbar.ToolbarRuleOps;

namespace ToSic.Sxc.Edit.Toolbar
{
    public partial class ToolbarBuilder
    {

        [PrivateApi("WIP v15.04")]
        public IToolbarBuilder Info(
            string noParamOrder = Eav.Parameters.Protector,
            object note = default,
            string link = default,
            object ui = default,
            object parameters = default,
            string operation = default
        ) => InfoLikeButton(
            noParamOrder: noParamOrder,
            commandName: "info",
            ui: ui,
            uiMerge: note is string ? new { note = new { note} } as object : new { note },
            parameters: parameters,
            parametersMerge: link != default ? new { link, } : null,
            operation: operation
        );

        private IToolbarBuilder InfoLikeButton(
            string noParamOrder,
            string commandName,
            object ui,
            object uiMerge,
            object parameters,
            object parametersMerge,
            string operation
        )
        {
            Eav.Parameters.Protect(noParamOrder, "See docs");
            var paramsWithMessage = new ObjectToUrl().SerializeWithChild(parameters, parametersMerge);
            var pars = PrecleanParams(operation: operation, defOp: OprNone, ui: ui, uiMerge: uiMerge, uiMergePrefix: null, parameters: paramsWithMessage, prefill: null);
            return EntityRule(commandName, null, pars).Builder;

        }
    }
}
