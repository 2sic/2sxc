using System;
using ToSic.Eav.Data;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Edit.Toolbar
{
    public class ToolbarRuleBase: HybridHtmlString
    {
        protected ToolbarRuleBase(): base(null) {}

        protected ToolbarRuleBase(string rule): this()
        {
            Rule = rule;
        }

        public override string ToString() => Rule;

        public IEntity Target { get; set; }
        public bool TargetRequired { get; set; }

        public bool Prepared { get; set; }

        public virtual string Rule
        {
            get
            {
                if (Prepared) return _rule;
                Prepared = true;
                _rule = BuildRule();
                return _rule;
            }
            set
            {
                _rule = value;
                Prepared = true;
            }
        }

        protected virtual string BuildRule() => string.Empty;

        private string _rule;

    }
}
