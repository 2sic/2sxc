using ToSic.Eav.DataSource.Internal.Query;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Apps.Internal.Assets;
using static ToSic.Sxc.Blocks.Internal.ViewConstants;
using IEntity = ToSic.Eav.Data.IEntity;

namespace ToSic.Sxc.Blocks.Internal;

[PrivateApi("Internal implementation - don't publish")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class View(
    IEntity templateEntity,
    string[] languageCodes,
    ILog parentLog,
    Generator<QueryDefinitionBuilder> qDefBuilder,
    bool isReplaced = false)
    : EntityBasedWithLog(templateEntity, languageCodes, parentLog, "Sxc.View"), IView
{
    private IEntity GetBestRelationship(string key) => Entity.Children(key).FirstOrDefault();


    public string Name => GetThis("unknown name");

    public string Identifier => GetThis("");
        
    public string Icon => GetThis("");

    public string Path => GetThis("");

    public string ContentType => Get(FieldContentType, "");

    public IEntity ContentItem => GetBestRelationship(FieldContentDemo);

    public string PresentationType => Get(FieldPresentationType, "");

    public IEntity PresentationItem => GetBestRelationship(FieldPresentationItem);

    public string HeaderType => Get(FieldHeaderType, "");

    public IEntity HeaderItem => GetBestRelationship(FieldHeaderItem);

    public string HeaderPresentationType => Get(FieldHeaderPresentationType, "");

    public IEntity HeaderPresentationItem => GetBestRelationship(FieldHeaderPresentationItem);

    public string Type => GetThis("");

    [PrivateApi]
    internal string GetTypeStaticName(string groupPart) =>
        groupPart.ToLowerInvariant() switch
        {
            ViewParts.ContentLower => ContentType,
            ViewParts.PresentationLower => PresentationType,
            ViewParts.ListContentLower => HeaderType,
            ViewParts.ListPresentationLower => HeaderPresentationType,
            _ => throw new NotSupportedException("Unknown group part: " + groupPart)
        };

    public bool IsHidden => GetThis(false);

    public bool IsShared => _isShared ??= AppAssets.IsShared(Get(FieldLocation, AppAssets.AppInSite));
    private bool? _isShared;

    public bool UseForList => GetThis(false);
    public bool PublishData => GetThis(false);
    public string StreamsToPublish => GetThis("");

    public IEntity QueryRaw => QueryInfo.Entity;

    public QueryDefinition Query => QueryInfo.Definition;

    private (IEntity Entity, QueryDefinition Definition) QueryInfo => _queryInfo.Get(() =>
    {
        var queryRaw = GetBestRelationship(FieldPipeline);
        var query = queryRaw != null
            ? (qDefBuilder?.New().Create(queryRaw, Entity.AppId)
               ?? throw new ArgumentException(
                   @"Query Definition builder is null. View is probably from PiggyBack cache. To use it, you must first Recreate it with the WorkViews",
                   nameof(qDefBuilder)))
            : null;
        return (queryRaw, query);
    });

    private readonly GetOnce<(IEntity Entity, QueryDefinition Definition)> _queryInfo = new();

    public string UrlIdentifier => Get(FieldNameInUrl, ""); // Entity.Value<string>(FieldNameInUrl);

    /// <summary>
    /// Returns true if the current template uses Razor
    /// </summary>
    public bool IsRazor => Type == TypeRazorValue;

    public string Edition { get; set; }

    public string EditionPath { get; set; }

    public IEntity Resources => GetBestRelationship(FieldResources);

    public IEntity Settings => GetBestRelationship(FieldSettings);

    /// <inheritdoc />
    public bool SearchIndexingDisabled => Get(FieldSearchDisabled, false);

    /// <inheritdoc />
    public string ViewController => Get(FieldViewController, "");

    /// <inheritdoc />
    public string SearchIndexingStreams => Get(FieldSearchStreams, "");

    public bool IsReplaced => isReplaced;
}