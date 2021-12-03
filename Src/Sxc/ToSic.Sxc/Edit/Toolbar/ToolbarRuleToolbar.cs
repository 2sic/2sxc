namespace ToSic.Sxc.Edit.Toolbar
{
    internal class ToolbarRuleToolbar: ToolbarRule
    {
        internal const string Empty = "empty";
        internal const string Default = "default";

        public ToolbarRuleToolbar(string template = "", string ui = ""/*, string parameters = ""*/): base("toolbar", ui: ui/*, parameters: parameters*/)
        {
            CommandValue = template;
        }
    }
}
