using ToSic.Eav.Documentation;
using ToSic.Sxc.Edit;
using ToSic.Sxc.Edit.Toolbar;

namespace ToSic.Sxc.Services
{
    [PrivateApi("Hide implementation")]
    public class ToolbarService: IToolbarService
    {
        public IToolbarBuilder Default(
            string noParamOrder = Eav.Parameters.Protector,
            string ui = "") 
            => NewBuilder(noParamOrder, ToolbarRuleToolbar.Default, ui);

        public IToolbarBuilder Empty(
            string noParamOrder = Eav.Parameters.Protector,
            string ui = "") 
            => NewBuilder(noParamOrder, ToolbarRuleToolbar.Empty, ui);

        public IToolbarBuilder Metadata(object target, string contentTypes) => Empty().Metadata(target, contentTypes);

        private IToolbarBuilder NewBuilder(string noParamOrder, string toolbarTemplate, string ui)
        {
            Eav.Parameters.ProtectAgainstMissingParameterNames(noParamOrder, "Toolbar", $"{nameof(ui)}");
            return new ToolbarBuilder()
                .Add(new ToolbarRuleToolbar(toolbarTemplate, ui: ui));
        }

    }
}
