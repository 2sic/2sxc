using ToSic.Eav.Data.Sys.ValueConverter;
using ToSic.Eav.Metadata;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Images.Sys;

namespace ToSic.Sxc.Data.Sys.Fields;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class Field(ITypedItem parent, string name, ICodeDataFactory cdf) : IField
{
    /// <inheritdoc />
    public string Name { get; } = name;

    public ITypedItem Parent { get; } = parent;

    /// <inheritdoc />
    [PrivateApi("Was public till 16.03, but don't think it should be surfaced...")]
    public object? Raw
    {
        get => _raw.Get(() => Parent.Get(Name, required: false));
        // Reason is for special edge cases like in School-Sys where we must process
        // the string before using it for Cms.Html(...)
        // Removed v22 2026-08-11 2dm - monitor; #CleanupV23
        //set => _raw.Set(value);
    }
    private readonly LazyGetAndReset<object?> _raw = new();


    /// <inheritdoc />
    [PrivateApi("Was public till 16.03, but don't think it should be surfaced...")]
    public object? Value
    {
        get => _value.Get(() => Url ?? Raw);
        // Removed v22 2026-08-11 2dm - monitor; #CleanupV23
        //set => _value.Set(value);
    }
    private readonly LazyGetAndReset<object?> _value = new();

    /// <inheritdoc />
    public string? Url
    {
        get => _url.Get(() => Parent.Url(Name));
        // Removed v22 2026-08-11 2dm - monitor; #CleanupV23
        //set => _url.Set(value);
    }
    private readonly LazyGetAndReset<string?> _url = new();


    protected IMetadata? MetadataOfValue => _itemMd.Get(() =>
    {
        // Check if string is valid, and also a valid reference like file:742
        if (Raw is not string rawString
            || string.IsNullOrWhiteSpace(rawString)
            || !ValueConverterBase.CouldBeReference(rawString))
            return null;

        // Get AppState to retrieve metadata - but exit early if we don't have it
        if ((cdf as ICodeDataFactoryDeepWip)?.AppReaderOrNull is not { } appReader)
            return null;

        var mdOf = appReader.Metadata.GetMetadataOf(TargetTypes.CmsItem, rawString, title: "");
        cdf.GetService<IImageMetadataRecommendationsService>()
            .SetImageRecommendations(mdOf, Url); // needs the url so it can check if we use image recommendations
        return mdOf;
    });
    private readonly LazyGet<IMetadata?> _itemMd = new();

    [PrivateApi("Internal use only, may change at any time")]
    public ImageDecorator? ImageDecoratorOrNull =>
        _imgDec.Get(() => ImageDecorator.GetOrNull(this, cdf.Dimensions));
    private readonly LazyGet<ImageDecorator?> _imgDec = new();

    IMetadata IHasMetadata.Metadata => MetadataOfValue!;
}