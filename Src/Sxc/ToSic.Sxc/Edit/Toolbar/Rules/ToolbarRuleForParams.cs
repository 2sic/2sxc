namespace ToSic.Sxc.Edit.Toolbar;

internal class ToolbarRuleForParams: ToolbarRuleForEntity
{
    public const string CommandName = "params";

    internal ToolbarRuleForParams(
        ToolbarRuleForParams original,
        object target,
        string ui = null,
        string parameters = null
    ) : this(target, ui ?? original?.Ui, parameters ?? original?.Parameters, original?.Context, original?.DecoHelper)
    {}

    internal ToolbarRuleForParams(
        object target,
        string ui = null,
        string parameters = null,
        ToolbarContext context = null,
        ToolbarButtonDecoratorHelper decoHelper = null) 
        : base(CommandName, target, null, null, ui: ui, parameters: parameters, context, decoHelper)
    {

    }
}