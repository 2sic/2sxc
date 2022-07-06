namespace ToSic.Sxc.Edit.Toolbar
{
    public class ToolbarRuleContext: ToolbarRuleTargeted
    {
        internal const string CommandName = "context";

        public ToolbarRuleContext(
            object target,
            ToolbarContext context = null,
            ToolbarButtonDecoratorHelper decoHelper = null
        ) : base(target, CommandName, null, null, null, context, decoHelper)
        {
        }
    }
}
