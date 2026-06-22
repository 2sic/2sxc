namespace ToSic.Sxc.Edit.Toolbar.Sys.Rules;

[ShowApiWhenReleased(ShowApiMode.Never)]
internal class ToolbarRuleContext(
    ToolbarButtonDecoratorHelper decoHelper,
    object? target,
    ToolbarContext? context = null
    )
    : ToolbarRuleTargeted(target: target, command: CommandName, decoHelper: decoHelper, ui: null, parameters: null, operation: null, context: context)
{
    internal const string CommandName = "context";
}