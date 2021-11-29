using System;
using ToSic.Eav.Data;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Edit.Toolbar
{
    public class ToolbarRuleBase: IString
    {
        protected ToolbarRuleBase() {}

        protected ToolbarRuleBase(string rule)
        {
            Rule = rule;
        }

        public IEntity Target { get; set; }
        public bool TargetRequired { get; set; }

        public bool Prepared { get; set; }

        public string Command { get; set; }

        public string CommandParams { get; set; }

        public string UiParams { get; set; }

        public string Rule
        {
            get
            {
                if (Prepared) return _rule;
                Prepared = true;
                _rule = PrepareRule();
                return _rule;
            }
            set
            {
                _rule = value;
                Prepared = true;
            }
        }

        private string PrepareRule()
        {
            throw new NotImplementedException();
        }

        private string _rule;

    }
}
