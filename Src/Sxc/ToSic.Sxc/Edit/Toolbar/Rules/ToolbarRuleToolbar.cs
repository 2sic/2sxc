namespace ToSic.Sxc.Edit.Toolbar
{
    internal class ToolbarRuleToolbar: ToolbarRule
    {
        internal const string Empty = "empty";
        internal const string Default = "default";

        public ToolbarRuleToolbar(string template = "", string ui = ""): base("toolbar", ui: ui)
        {
            CommandValue = template;
        }
    }
}
