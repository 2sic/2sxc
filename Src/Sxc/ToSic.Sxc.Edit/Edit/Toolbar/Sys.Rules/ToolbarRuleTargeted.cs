using ToSic.Sxc.Web.Sys.Url;
using ToSic.Sys.Utils;
using static ToSic.Sxc.Edit.Toolbar.Sys.Rules.ToolbarRuleOps;

namespace ToSic.Sxc.Edit.Toolbar.Sys.Rules;

/// <summary>
/// A toolbar rule for a specific target
/// </summary>
internal abstract class ToolbarRuleTargeted: ToolbarRule
{
    protected ToolbarRuleTargeted(
        object? target, 
        string command, 
        ToolbarButtonDecoratorHelper decoHelper,
        string? ui = null, 
        string? parameters = null, 
        char? operation = null,
        ToolbarContext? context = null
    ) : base(command: command, ui: ui, parameters: parameters, operation: operation, operationCode: target as string, context: context)
    {
        Target = target;
        DecoHelper = decoHelper;

        var operationCode = target as string;
        // Special case, if target is "-" or "remove" etc.
        if (!operationCode.HasValue())
            return;

        var targetCouldBeOperation = ToolbarRuleOperation.Pick(operationCode, OprUnknown);
        if (targetCouldBeOperation == (char)OprUnknown)
            return;

        Target = null;
        Operation = targetCouldBeOperation;
    }

    internal object? Target { get; init; }

    protected readonly ToolbarButtonDecoratorHelper DecoHelper;

    public override string GeneratedUiParams()
        => UrlParts.ConnectParameters(UiParamsFromDecorator, base.GeneratedUiParams());


    #region Decorators

    protected virtual string DecoratorTypeName => field
        ??= (Target as ICanBeEntity)?.Entity.Type.Name
        ?? "";

    private ToolbarButtonDecorator? Decorator => _decorator.Get(() =>
        DecoratorTypeName.HasValue()
            ? DecoHelper.GetDecorator(Context, DecoratorTypeName, Command)
            : null
    );
    private readonly GetOnce<ToolbarButtonDecorator?> _decorator = new();

    private string UiParamsFromDecorator => field ??= Decorator?.AllRules() ?? "";

    #endregion
}