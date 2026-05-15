using System.Numerics;
using ToSic.Eav.Data.Build.Sys;
using ToSic.Sxc.Web.Sys.Url;

namespace ToSic.Sxc.Edit.Toolbar.Sys.Rules;

[ShowApiWhenReleased(ShowApiMode.Never)]
internal class ToolbarRuleForEntity: ToolbarRuleTargeted
{
    internal ToolbarRuleForEntity(
        string commandName,
        object? target = null,   // IEntity, DynEntity or int
        char? operation = null,
        string? contentType = null,
        string? ui = null, 
        string? parameters = null,
        ToolbarContext? context = null,
        ToolbarButtonDecoratorHelper? decoHelper = null,
        string[]? propsSkip = null,
        string[]? propsKeep = null
    ) : base(target, commandName, operation: operation, ui: ui, parameters: parameters, context: context, decoHelper: decoHelper)
    {
        if (target is int intTarget)
            EditInfo!.entityId = intTarget;
        if (contentType != null)
            EditInfo!.contentType = contentType;

        // new 21.08 2dm 2026-05-15 skip content-type if it's a virtual content-type
        if (EditInfo!.contentType == DataAssemblerExtensions.FakeEntityContentType)
            EditInfo.contentType = null;

        if (propsSkip != null)
            _urlValueFilterNames = new(true, propsSkip);
        else if (propsKeep != null)
            _urlValueFilterNames = new(false, propsKeep);
    }

    /// <summary>
    /// The filter for what entity properties to keep in the params. By default, keep all.
    /// </summary>
    private readonly UrlValueFilterNames _urlValueFilterNames = new(true, []);


    protected IEntity? TargetEntity => _entity.Get(() => Target as IEntity ?? (Target as ICanBeEntity)?.Entity);
    private readonly GetOnce<IEntity?> _entity = new();

    [field: AllowNull, MaybeNull]
    internal EntityEditInfo EditInfo => field ??= new(TargetEntity);

    protected override string DecoratorTypeName => TargetEntity?.Type?.Name ?? "";

    public override string GeneratedCommandParams()
        => UrlParts.ConnectParameters(EntityParamsList(), base.GeneratedCommandParams());

    protected string? EntityParamsList()
    {
        var obj2Url = new ObjectToUrl(null, [_urlValueFilterNames]);
        return obj2Url.Serialize(EditInfo);
    }
}