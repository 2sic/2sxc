namespace ToSic.Sxc.Edit.Toolbar
{
    internal class ToolbarRuleForParams: ToolbarRuleForEntity
    {
        public const string CommandName = "params";

        internal ToolbarRuleForParams(
            object target,
            string ui = null,
            string parameters = null,
            ToolbarContext context = null,
            ToolbarButtonDecoratorHelper helper = null) 
            : base(CommandName, target, null, null, null, ui, parameters, context, helper)
        {

        }
    }
}
