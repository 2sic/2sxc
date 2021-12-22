using ToSic.Sxc.Web;

namespace ToSic.Sxc.Edit.Toolbar
{
    public class ToolbarRuleBase: HybridHtmlString
    {
        protected ToolbarRuleBase(): base(string.Empty) {}

        protected ToolbarRuleBase(string rule): base(rule) { }

    }
}
