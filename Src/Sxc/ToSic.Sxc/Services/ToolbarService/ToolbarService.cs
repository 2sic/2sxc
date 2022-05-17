using ToSic.Eav.Documentation;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Edit;
using ToSic.Sxc.Edit.Toolbar;

namespace ToSic.Sxc.Services
{
    [PrivateApi("Hide implementation")]
    public class ToolbarService: IToolbarService
    {
        public IToolbarBuilder Default(
            string noParamOrder = Eav.Parameters.Protector,
            string ui = null
        ) => NewBuilder(noParamOrder, ToolbarRuleToolbar.Default, ui, null);

        public IToolbarBuilder Empty(
            string noParamOrder = Eav.Parameters.Protector,
            string ui = null
        ) => NewBuilder(noParamOrder, ToolbarRuleToolbar.Empty, ui, null);

        public IToolbarBuilder Metadata(
            object target,
            string contentTypes,
            string noParamOrder = Eav.Parameters.Protector,
            string ui = null,
            string parameters = null
        ) => Empty().Metadata(target, contentTypes, ui: ui, parameters: parameters);

        private IToolbarBuilder NewBuilder(string noParamOrder, string toolbarTemplate, string ui, string context)
        {
            Eav.Parameters.ProtectAgainstMissingParameterNames(noParamOrder, "Toolbar", $"{nameof(ui)}");
            var tlb = new ToolbarBuilder()
                .Add(new ToolbarRuleToolbar(toolbarTemplate, ui: ui));
            if (context.HasValue())
                tlb = tlb.Add(new ToolbarRuleGeneric($"context?{context}"));
            return tlb;
        }

    }
}
