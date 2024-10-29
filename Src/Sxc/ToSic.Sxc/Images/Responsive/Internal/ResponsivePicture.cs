using ToSic.Eav.Plumbing;
using ToSic.Lib.Helpers;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;
using ToSic.Razor.Markup;
using ToSic.Sxc.Services;
using static ToSic.Sxc.Configuration.Internal.SxcFeatures;

namespace ToSic.Sxc.Images.Internal;

/// <remarks>
/// Must be public, otherwise it breaks in dynamic use :(
/// </remarks>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class ResponsivePicture: ResponsiveBase, IResponsivePicture
{
    internal ResponsivePicture(ImageService imgService, IPageService pageService, ResponsiveSpecs specs, ILog parentLog)
        : base(imgService, pageService, specs, parentLog, "Picture")
    { }

    public Picture Picture => _picTag.Get(() =>
    {
        var pic = ToSic.Razor.Blade.Tag.Picture(Sources, Img);
        var picSpecs = Tweaker.Pic;
        pic = AddAttributes(pic, picSpecs.Attributes);
        if (Tweaker.Pic.Class.HasValue()) pic = pic.Class(Tweaker.Pic.Class);
        if (TryGetAttribute(picSpecs.Attributes, Recipe.SpecialPropertyStyle, out var style))
            pic = pic.Style(style);
        return pic;
    });
    private readonly GetOnce<Picture> _picTag = new();

    protected override IHtmlTag GetOutermostTag() => Picture;

    public TagList Sources => _sourceTags.Get(() => SourceTagsInternal(Target.Link.Url, Settings));
    private readonly GetOnce<TagList> _sourceTags = new();

    private TagList SourceTagsInternal(string url, IResizeSettings resizeSettings)
    {
        var logOrNull = ImgService.Debug ? Log : null;
        var l = logOrNull.Fn<TagList>();
        // Check formats
        var defFormat = ImgService.GetFormat(url);
        if (defFormat == null)
            return l.Return(ToSic.Razor.Blade.Tag.TagList(), "no format");

        // Determine if we have many formats, otherwise just use the current one
        var formats = defFormat.ResizeFormats.Any()
            ? defFormat.ResizeFormats
            : [defFormat];
            
        var useMultiSrcSet = ImgService.Features.IsEnabled(ImageServiceMultipleSizes.NameId);

        l.A($"{nameof(formats)}: {formats.Count}, {nameof(useMultiSrcSet)}: {useMultiSrcSet}");

        // Generate Meta Tags
        var sources = formats
            .Select(resizeFormat =>
            {
                // We must copy the settings, because we change them and this shouldn't affect anything else
                var formatSettings = new ResizeSettings(resizeSettings, format: resizeFormat != defFormat ? resizeFormat.Format : null);
                var srcSet = useMultiSrcSet
                    ? ImgLinker.SrcSet(url, formatSettings, SrcSetType.Source, Target.HasMdOrNull)
                    : ImgLinker.ImageOnly(url, formatSettings, Target.HasMdOrNull).Url;
                var source = ToSic.Razor.Blade.Tag.Source().Type(resizeFormat.MimeType).Srcset(srcSet);
                if (!string.IsNullOrEmpty(Sizes)) source.Sizes(Sizes);
                return source;
            });
        var result = ToSic.Razor.Blade.Tag.TagList(sources);
        return l.Return(result, $"{result.Count()}");
    }

}