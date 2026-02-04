using ToSic.Eav.DataSource.Sys.Query;
using ToSic.Eav.Metadata;
using static ToSic.Sxc.Blocks.Sys.Views.ViewConstants;


namespace ToSic.Sxc.Blocks.Sys.Views;

[PrivateApi("Internal implementation - don't publish")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public record View : ViewConfiguration, IView
{
    public View(IEntity templateEntity,
        string?[] languageCodes,
        Generator<QueryDefinitionBuilder>? qDefBuilder,
        bool isReplaced = false) : base(templateEntity, languageCodes)
    {
        //LookupLanguages = languageCodes;
        _qDefBuilder = qDefBuilder;
        IsReplaced = isReplaced;
    }

    private IEntity? Child(string key)
        => Entity.Children(key).FirstOrDefault();


    //public string Name => GetThis("unknown name");

    //public string Identifier => GetThis("");
        
    //public string Icon => GetThis("");

    //public string Path => GetThis("");

    //public string ContentType => Get(FieldContentType, "");

    //public IEntity? ContentItem => Child(FieldContentDemo);

    //public string PresentationType => Get(FieldPresentationType, "");

    //public IEntity? PresentationItem => Child(FieldPresentationItem);

    //public string HeaderType => Get(FieldHeaderType, "");

    //public IEntity? HeaderItem => Child(FieldHeaderItem);

    //public string HeaderPresentationType => Get(FieldHeaderPresentationType, "");

    //public IEntity? HeaderPresentationItem => Child(FieldHeaderPresentationItem);

    //public string Type => GetThis("");

    //public bool IsHidden => GetThis(false);

    //public bool IsShared => _isShared ??= AppAssetsHelpers.IsShared(Get(FieldLocation, AppAssetsHelpers.AppInSite));
    //private bool? _isShared;

    //public bool UseForList => GetThis(false);

    public IEntity? QueryRaw => QueryInfo.QueryEntity;

    public QueryDefinition? Query => QueryInfo.Definition;

    private (IEntity? QueryEntity, QueryDefinition? Definition) QueryInfo => _queryInfo.Get(() =>
    {
        var queryRaw = Child(FieldPipeline);
        var query = queryRaw != null
            ? (_qDefBuilder ?? throw new ArgumentException(
                @"Query Definition builder is null. View is probably from PiggyBack cache. To use it, you must first Recreate it with the WorkViews",
                nameof(_qDefBuilder))
            ).New().Create(queryRaw, Entity.AppId)
            : null;
        return (queryRaw, query);
    });

    private readonly GetOnce<(IEntity? QueryEntity, QueryDefinition? Definition)> _queryInfo = new();
    private readonly Generator<QueryDefinitionBuilder>? _qDefBuilder;


    //public string UrlIdentifier => Get(FieldNameInUrl, "");

    /// <summary>
    /// Returns true if the current template uses Razor
    /// </summary>
    public bool IsRazor => Type == TypeRazorValue;

    public string? Edition { get; set; }

    public string? EditionPath { get; set; }

    //public IEntity? Resources => Child(FieldResources);

    //public IEntity? Settings => Child(FieldSettings);

    /// <inheritdoc />
    //public bool SearchIndexingDisabled => Get(FieldSearchDisabled, false);

    ///// <inheritdoc />
    //public string ViewController => Get(FieldViewController, "");

    ///// <inheritdoc />
    //public string SearchIndexingStreams => Get(FieldSearchStreams, "");

    public bool IsReplaced { get; }

    public IMetadata Metadata => Entity.Metadata;
}