namespace ToSic.Sxc.Edit.Toolbar.Sys.Rules;

internal class ToolbarRuleForParams(
    object? target,
    ToolbarButtonDecoratorHelper decoHelper,
    string? ui = null,
    string? parameters = null,
    ToolbarContext? context = null
)
    : ToolbarRuleForEntity(commandName: CommandName, decoHelper: decoHelper, target: target, operation: null, contentType: null, ui: ui, parameters: parameters, context: context)
{
    public const string CommandName = "params";

    //internal ToolbarRuleForParams(
    //    ToolbarRuleForParams original,
    //    object target,
    //    string? ui = null,
    //    string? parameters = null
    //) : this(target, ui ?? original?.Ui, parameters ?? original?.Parameters, original?.Context, original?.DecoHelper)
    //{}
}