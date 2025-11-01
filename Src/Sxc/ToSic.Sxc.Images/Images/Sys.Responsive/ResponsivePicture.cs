using ToSic.Razor.Blade;
using ToSic.Razor.Html5;
using ToSic.Razor.Markup;
using ToSic.Sxc.Images.Sys;
using ToSic.Sxc.Images.Sys.ResizeSettings;
using ToSic.Sxc.Services;
using ToSic.Sxc.Sys.Configuration;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Images;

/// <remarks>
/// Must be public, otherwise it breaks in dynamic use :(
/// </remarks>
[ShowApiWhenReleased(ShowApiMode.Never)]
public record ResponsivePicture: ResponsiveBase, IResponsivePicture
{
    internal ResponsivePicture(ImageService imgService, IPageService pageService, ResponsiveSpecs specs, ILog parentLog)
        : base(imgService, pageService, specs, parentLog, "Picture")
    { }

    [field: AllowNull, MaybeNull]
    public Picture Picture => field ??= GeneratePicture();

    private Picture GeneratePicture()
    {
        var pic = ToSic.Razor.Blade.Tag.Picture(Sources, Img);
        var picSpecs = Tweaker.Pic;
        pic = AddAttributes(pic, picSpecs.Attributes);
        if (Tweaker.Pic.Class.HasValue())
            pic = pic.Class(Tweaker.Pic.Class);
        if (TryGetAttribute(picSpecs.Attributes, Recipe.SpecialPropertyStyle, out var style))
            pic = pic.Style(style);
        return pic;
    }

    /// <summary>
    /// Necessary so it also works with ToString() otherwise the record will show the JSON.
    /// </summary>
    public override string ToString() => ToHtmlString();

    protected override IHtmlTag GetOutermostTag() => Picture;

    [field: AllowNull, MaybeNull]
    public TagList Sources => field ??= SourceTagsInternal(Target.Link.Url, Settings);

    private TagList SourceTagsInternal(string? url, IResizeSettings resizeSettings)
    {
        var logOrNull = ImgService.Debug ? Log : null;
        var l = logOrNull.Fn<TagList>();
        // Check formats
        var defFormat = ImgService.GetFormat(url);

        // Determine if we have many formats, otherwise just use the current one
        var formats = defFormat.ResizeFormats.Any()
            ? defFormat.ResizeFormats
            : [defFormat];
            
        var useMultiSrcSet = ImgService.Features.IsEnabled(SxcFeatures.ImageServiceMultipleSizes.NameId);

        l.A($"{nameof(formats)}: {formats.Count}, {nameof(useMultiSrcSet)}: {useMultiSrcSet}");

        // Generate Meta Tags
        var sources = formats
            .Select(resizeFormat =>
            {
                // We must copy the settings, because we change them and this shouldn't affect anything else
                var formatSettings = new ResizeSettings(resizeSettings, format: resizeFormat != defFormat ? resizeFormat.Format : null);
                var srcSet = useMultiSrcSet
                    ? ImgService.ImgLinker.SrcSet(url, formatSettings, SrcSetType.Source, Target.HasMdOrNull, overrideFramework: ImgService.OverrideCssFramework)
                    : ImgService.ImgLinker.ImgResizeSettings(url, formatSettings, Target.HasMdOrNull, overrideFramework: ImgService.OverrideCssFramework).Url;
                var source = ToSic.Razor.Blade.Tag.Source().Type(resizeFormat.MimeType).Srcset(srcSet);
                if (!string.IsNullOrEmpty(Sizes)) source.Sizes(Sizes);
                return source;
            });
        var result = ToSic.Razor.Blade.Tag.TagList(sources);
        return l.Return(result, $"{result.Count()}");
    }

}