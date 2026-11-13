using ToSic.Eav.Apps.Sys;
using ToSic.Sxc.Apps.Sys.Assets;
using static ToSic.Sxc.Blocks.Sys.Views.ViewConstants;


namespace ToSic.Sxc.Blocks.Sys.Views;

[PrivateApi("Internal implementation - don't publish")]
[ShowApiWhenReleased(ShowApiMode.Never)]
[ModelSpecs(ContentType = ContentTypeNameId)]
public record ViewConfiguration : ModelFromEntityFull
{
    public const string ContentTypeNameId = AppConstants.TemplateContentType;
    public const string ContentTypeName = AppConstants.TemplateContentType;

    /// <summary>
    /// Empty constructor for DI and factory.
    /// </summary>
    [PrivateApi]
    public ViewConfiguration() {}

    /// <summary>
    /// Internal constructor for quick proper create.
    /// </summary>
    /// <param name="templateEntity"></param>
    /// <param name="languageCodes"></param>
    [PrivateApi]
    public ViewConfiguration(IEntity templateEntity, string?[] languageCodes) : base(templateEntity, languageCodes)
    { }

    private IEntity? Child(string key)
        => Entity.Children(key).FirstOrDefault();


    public string Name => GetThis("unknown name");

    public string Identifier => GetThis("");
        
    public string Icon => GetThis("");

    public string Path => GetThis("");

    public string ContentType => Get(FieldContentType, "");

    public IEntity? ContentItem => Child(FieldContentDemo);

    public string PresentationType => Get(FieldPresentationType, "");

    public IEntity? PresentationItem => Child(FieldPresentationItem);

    public string HeaderType => Get(FieldHeaderType, "");

    public IEntity? HeaderItem => Child(FieldHeaderItem);

    public string HeaderPresentationType => Get(FieldHeaderPresentationType, "");

    public IEntity? HeaderPresentationItem => Child(FieldHeaderPresentationItem);

    public string Type => GetThis("");

    public bool IsHidden => GetThis(false);

    public bool IsShared => _isShared ??= AppAssetsHelpers.IsShared(Get(FieldLocation, AppAssetsHelpers.AppInSite));
    private bool? _isShared;

    public bool UseForList => GetThis(false);


    public string UrlIdentifier => Get(FieldNameInUrl, "");

    public IEntity? Resources => Child(FieldResources);

    public IEntity? Settings => Child(FieldSettings);

    /// <inheritdoc />
    public bool SearchIndexingDisabled => Get(FieldSearchDisabled, false);

    /// <inheritdoc />
    public string ViewController => Get(FieldViewController, "");

    /// <inheritdoc />
    public string SearchIndexingStreams => Get(FieldSearchStreams, "");

}