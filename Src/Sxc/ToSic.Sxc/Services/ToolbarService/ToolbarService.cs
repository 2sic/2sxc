using ToSic.Eav.Documentation;
using ToSic.Sxc.Edit.Toolbar;

namespace ToSic.Sxc.Services
{
    [PrivateApi("Hide implementation")]
    public class ToolbarService: IToolbarService
    {
        public ToolbarBuilder Default(string noParamOrder = Eav.Parameters.Protector, string ui = "") 
            => NewBuilder(noParamOrder, ToolbarRuleToolbar.Default, ui);

        public ToolbarBuilder Empty(string noParamOrder = Eav.Parameters.Protector, string ui = "") 
            => NewBuilder(noParamOrder, ToolbarRuleToolbar.Empty, ui);


        private ToolbarBuilder NewBuilder(string noParamOrder, string toolbarTemplate, string ui)
        {
            Eav.Parameters.ProtectAgainstMissingParameterNames(noParamOrder, "Toolbar", $"{nameof(ui)}");
            return new ToolbarBuilder()
                .Add(new ToolbarRuleToolbar(toolbarTemplate, ui: ui));
        }
    }
}
