namespace ToSic.Sxc.Edit.Toolbar;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class ToolbarRuleContext: ToolbarRuleTargeted
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