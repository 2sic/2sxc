namespace ToSic.Sxc.Edit.Toolbar
{
    internal class ToolbarRuleCustom: ToolbarRuleTargeted
    {
        public ToolbarRuleCustom(
            object target, 
            string command, 
            string ui = null, 
            string parameters = null, 
            char? operation = null, 
            ToolbarContext context = null,
            ToolbarButtonDecoratorHelper helper = null) 
            : base(target, command, ui, parameters, operation, context, helper)
        {
        }
    }
}
