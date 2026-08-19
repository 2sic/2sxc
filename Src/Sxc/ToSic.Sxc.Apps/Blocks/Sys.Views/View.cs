using ToSic.Eav.DataSource.Query.Sys;
using ToSic.Eav.Metadata;
using static ToSic.Sxc.Blocks.Sys.Views.ViewConstants;


namespace ToSic.Sxc.Blocks.Sys.Views;

[PrivateApi("Internal implementation - don't publish")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public record View : ViewConfiguration, IView
{
    public View(IEntity templateEntity,
        string?[] languageCodes,
        Generator<QueryDefinitionFactory>? qDefBuilder,
        bool isReplaced = false) : base(templateEntity, languageCodes)
    {
        _qDefBuilder = qDefBuilder;
        IsReplaced = isReplaced;
    }

    private IEntity? Child(string key)
        => Entity.Children(key).FirstOrDefault();


    public IEntity? QueryRaw => QueryInfo.QueryEntity;

    public QueryDefinition? Query => QueryInfo.Definition;

    private (IEntity? QueryEntity, QueryDefinition? Definition) QueryInfo => _queryInfo.Get(() =>
    {
        var queryRaw = Child(FieldPipeline);
        var query = queryRaw != null
            ? (_qDefBuilder ?? throw new ArgumentException(
                @"Query Definition builder is null. View is probably from PiggyBack cache. To use it, you must first Recreate it with the WorkViews",
                nameof(_qDefBuilder))
            ).New().Create(Entity.AppId, queryRaw)
            : null;
        return (queryRaw, query);
    });

    private readonly LazyGet<(IEntity? QueryEntity, QueryDefinition? Definition)> _queryInfo = new();
    private readonly Generator<QueryDefinitionFactory>? _qDefBuilder;


    /// <summary>
    /// Returns true if the current template uses Razor
    /// </summary>
    public bool IsRazor => Type == TypeRazorValue;
    
    public string? Edition { get; set; }

    public string? EditionPath { get; set; }

    public bool IsReplaced { get; }

    public IMetadata Metadata => Entity.Metadata;
}