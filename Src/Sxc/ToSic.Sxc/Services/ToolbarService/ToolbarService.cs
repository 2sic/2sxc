using ToSic.Eav.Documentation;
using ToSic.Sxc.Edit;
using ToSic.Sxc.Edit.Toolbar;

namespace ToSic.Sxc.Services
{
    [PrivateApi("Hide implementation")]
    public class ToolbarService: IToolbarService
    {
        public IToolbarBuilder Default(
            string noParamOrder =
                "Rule: all params must be named (https://r.2sxc.org/named-params), Example: \'enable: true, version: 10\'",
            string ui = "") 
            => NewBuilder(noParamOrder, ToolbarRuleToolbar.Default, ui);

        public IToolbarBuilder Empty(
            string noParamOrder =
                "Rule: all params must be named (https://r.2sxc.org/named-params), Example: \'enable: true, version: 10\'",
            string ui = "") 
            => NewBuilder(noParamOrder, ToolbarRuleToolbar.Empty, ui);


        private IToolbarBuilder NewBuilder(string noParamOrder, string toolbarTemplate, string ui)
        {
            Eav.Parameters.ProtectAgainstMissingParameterNames(noParamOrder, "Toolbar", $"{nameof(ui)}");
            return new ToolbarBuilder()
                .Add(new ToolbarRuleToolbar(toolbarTemplate, ui: ui));
        }
    }
}
