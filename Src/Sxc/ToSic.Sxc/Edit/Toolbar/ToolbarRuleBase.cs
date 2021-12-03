using ToSic.Sxc.Web;

namespace ToSic.Sxc.Edit.Toolbar
{
    public class ToolbarRuleBase: HybridHtmlString
    {
        protected ToolbarRuleBase(): base(string.Empty) {}

        protected ToolbarRuleBase(string rule): base(rule) { }

        //public override string ToString() => base.ToString();

        //public virtual string Rule
        //{
        //    get => _optionalStoredRule ?? BuildRule();
        //}

        //protected virtual string BuildRule() => string.Empty;

        ///// <summary>
        ///// In case a string rule was set, it should be here. 
        ///// </summary>
        //private string _optionalStoredRule;

    }
}
