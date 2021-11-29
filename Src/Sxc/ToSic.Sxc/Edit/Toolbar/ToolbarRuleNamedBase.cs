namespace ToSic.Sxc.Edit.Toolbar
{
    public abstract class ToolbarRuleNamedBase: ToolbarRuleBase
    {
        public virtual string Operator => "";

        public virtual string Command => "edit";

        public virtual string CommandParams => "";

        public virtual string UiParams => "";

        protected override string BuildRule()
        {
            var result = Operator + Command;
            if (!string.IsNullOrWhiteSpace(UiParams)) result += "&" + UiParams;
            if (!string.IsNullOrWhiteSpace(CommandParams)) result += "?" + CommandParams;
            return result;
        }
    }
}
