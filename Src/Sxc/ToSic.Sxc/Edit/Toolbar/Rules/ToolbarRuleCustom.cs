namespace ToSic.Sxc.Edit.Toolbar
{
    internal class ToolbarRuleCustom: ToolbarRule
    {
        public ToolbarRuleCustom(
            string command, 
            string ui = null, 
            string parameters = null, 
            char? operation = null, 
            ToolbarContext context = null
        ) : base(command, ui: ui, parameters: parameters, operation: operation, context: context)
        {
        }
    }
}
