﻿using ToSic.Eav.Data.ValueConverter.Sys;
using ToSic.Eav.Metadata;
using ToSic.Lib.Helpers;
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
        // WIP 2023-10-28 2dm Experimental Setter #FieldSetExperimental
        // Reason is for special edge cases like in School-Sys where we must process
        // the string before using it for Cms.Html(...)
        set => _raw.Reset(value);
    }
    private readonly GetOnce<object?> _raw = new();


    /// <inheritdoc />
    [PrivateApi("Was public till 16.03, but don't think it should be surfaced...")]
    public object? Value
    {
        get => _value.Get(() => Url ?? Raw)!;
        // WIP 2023-10-28 2dm Experimental Setter #FieldSetExperimental
        set => _value.Reset(value);
    }
    private readonly GetOnce<object?> _value = new();

    /// <inheritdoc />
    public string? Url
    {
        get => _url.Get(() => Parent.Url(Name))!;
        // WIP 2023-10-28 2dm Experimental Setter #FieldSetExperimental
        set => _url.Reset(value);
    }
    private readonly GetOnce<string?> _url = new();

    /// <summary>
    /// The Dynamic metadata - probably used somewhere...?
    /// 2023-08-14 v16.03 removed by 2dm as never used; KISS
    /// ...but reactivated for some reason I don't know...
    /// 2025-06 2dm: It appears we reactivated it, because various older apps had code like this:
    /// `var altText = Text.First(post.Field("Image").Metadata.Description, post.Title);`
    /// Apps include Blog v5.3.1, News 5.2.3, ImageHotspot 3.2.2
    /// </summary>
    public ITypedMetadata Metadata => _dynMeta.Get(() => cdf.Metadata(MetadataOfValue!))!;
    private readonly GetOnce<ITypedMetadata> _dynMeta = new();


    private IMetadata? MetadataOfValue => _itemMd.Get(() =>
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
    private readonly GetOnce<IMetadata?> _itemMd = new();

    [PrivateApi("Internal use only, may change at any time")]
    public ImageDecorator? ImageDecoratorOrNull =>
        _imgDec.Get(() => ImageDecorator.GetOrNull(this, cdf.Dimensions));
    private readonly GetOnce<ImageDecorator?> _imgDec = new();

    IMetadata IHasMetadata.Metadata => MetadataOfValue!;
}