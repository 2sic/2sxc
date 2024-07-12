using ToSic.Eav.Metadata;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Images.Internal;

namespace ToSic.Sxc.Data.Internal;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class Field(ITypedItem parent, string name, CodeDataFactory cdf) : IField
{
    /// <inheritdoc />
    public string Name { get; } = name;

    public ITypedItem Parent { get; } = parent;

    /// <inheritdoc />
    [PrivateApi("Was public till 16.03, but don't think it should be surfaced...")]
    public object Raw
    {
        get => _raw.Get(() => Parent.Get(Name, required: false));
        // WIP 2023-10-28 2dm Experimental Setter #FieldSetExperimental
        // Reason is for special edge cases like in School-Sys where we must process
        // the string before using it for Cms.Html(...)
        set => _raw.Reset(value);
    }
    private readonly GetOnce<object> _raw = new();


    /// <inheritdoc />
    [PrivateApi("Was public till 16.03, but don't think it should be surfaced...")]
    public object Value
    {
        get => _value.Get(() => Url ?? Raw);
        // WIP 2023-10-28 2dm Experimental Setter #FieldSetExperimental
        set => _value.Reset(value);
    }
    private readonly GetOnce<object> _value = new();

    /// <inheritdoc />
    public string Url
    {
        get => _url.Get(() => Parent.Url(Name));
        // WIP 2023-10-28 2dm Experimental Setter #FieldSetExperimental
        set => _url.Reset(value);
    }
    private readonly GetOnce<string> _url = new();


    // 2023-08-14 v16.03 removed by 2dm as never used; KISS
    public IMetadata Metadata => _dynMeta.Get(() => new Metadata.Metadata(MetadataOfValue, cdf));
    private readonly GetOnce<IMetadata> _dynMeta = new();


    private IMetadataOf MetadataOfValue => _itemMd.Get(() =>
    {
        if (Raw is not string rawString || string.IsNullOrWhiteSpace(rawString))
            return null;
        var appState = cdf?.BlockOrNull?.Context?.AppState;
        var md = appState?.GetMetadataOf(TargetTypes.CmsItem, rawString, "");
        ImageDecorator.AddRecommendations(md, Url, cdf?._CodeApiSvc); // needs the url so it can check if we use image recommendations
        return md;
    });
    private readonly GetOnce<IMetadataOf> _itemMd = new();

    [PrivateApi("Internal use only, may change at any time")]
    public ImageDecorator ImageDecoratorOrNull =>
        _imgDec.Get(() => ImageDecorator.GetOrNull(this, cdf.Dimensions));
    private readonly GetOnce<ImageDecorator> _imgDec = new();

    IMetadataOf IHasMetadata.Metadata => MetadataOfValue;
}