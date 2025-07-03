namespace ToSic.Sxc.Edit.Toolbar.Sys.Rules;

[ShowApiWhenReleased(ShowApiMode.Never)]
internal class ToolbarRuleContext(
    object? target,
    ToolbarContext? context = null,
    ToolbarButtonDecoratorHelper? decoHelper = null)
    : ToolbarRuleTargeted(target, CommandName, null, null, null, context, decoHelper)
{
    internal const string CommandName = "context";
}