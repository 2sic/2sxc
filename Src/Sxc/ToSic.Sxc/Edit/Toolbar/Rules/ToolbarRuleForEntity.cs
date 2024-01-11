using ToSic.Lib.Helpers;
using ToSic.Sxc.Web;
using ToSic.Sxc.Web.Internal.Url;

namespace ToSic.Sxc.Edit.Toolbar;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class ToolbarRuleForEntity: ToolbarRuleTargeted
{
    public const string PrefixPrefill = "prefill:";
    public const string PrefixForm = "form:";
    public const string PrefixFilters = "filter:";

    internal ToolbarRuleForEntity(
        string commandName,
        object target = null,   // IEntity, DynEntity or int
        char? operation = null,
        string contentType = null,
        string ui = null, 
        string parameters = null,
        ToolbarContext context = null,
        ToolbarButtonDecoratorHelper decoHelper = null,
        string[] propsSkip = null,
        string[] propsKeep = null
    ) : base(target, commandName, operation: operation, ui: ui, parameters: parameters, context: context, decoHelper: decoHelper)
    {
        if (target is int intTarget) EditInfo.entityId = intTarget;
        if (contentType != null) EditInfo.contentType = contentType;

        if (propsSkip != null) _urlValueFilterNames = new UrlValueFilterNames(true, propsSkip);
        else if (propsKeep != null) _urlValueFilterNames = new UrlValueFilterNames(false, propsKeep);
    }

    /// <summary>
    /// The filter for what entity properties to keep in the params. By default, keep all.
    /// </summary>
    private readonly UrlValueFilterNames _urlValueFilterNames = new(true, Array.Empty<string>());


    protected IEntity TargetEntity => _entity.Get(() => Target as IEntity ?? (Target as ICanBeEntity)?.Entity);
    private readonly GetOnce<IEntity> _entity = new();

    internal EntityEditInfo EditInfo => _editInfo.Get(() => new EntityEditInfo(TargetEntity));
    private readonly GetOnce<EntityEditInfo> _editInfo = new();

    protected override string DecoratorTypeName => TargetEntity?.Type?.Name;

    public override string GeneratedCommandParams()
        => UrlParts.ConnectParameters(EntityParamsList(), base.GeneratedCommandParams());

    protected string EntityParamsList()
    {
        var obj2Url = new ObjectToUrl(null, new[] { _urlValueFilterNames });
        //var obj2Url = new ObjectToUrl(null, (n, v) => _urlValueFilter.FilterValues(n, v));
        return obj2Url.Serialize(EditInfo);
    }
}