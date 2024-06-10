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
    internal ResponsivePicture(ImageService imgService, IPageService pageService, ResponsiveParams callParams, ILog parentLog) : base(imgService, pageService, callParams, parentLog, "Picture")
    {
    }


    public Picture Picture => _picTag.Get(() =>
    {
        var pic = ToSic.Razor.Blade.Tag.Picture(Sources, Img);
        pic = AddAttributes(pic, Params.PictureAttributes);
        if (Params.PictureClass.HasValue()) pic = pic.Class(Params.PictureClass);
        if (TryGetAttribute(Params.PictureAttributes, Recipe.SpecialPropertyStyle, out var style))
            pic = pic.Style(style);
        return pic;
    });
    private readonly GetOnce<Picture> _picTag = new();

    protected override IHtmlTag GetOutermostTag() => Picture;

    public TagList Sources => _sourceTags.Get(() => SourceTagsInternal(Params.Link.Url, Settings));
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
                    ? ImgLinker.SrcSet(url, formatSettings, SrcSetType.Source, Params.HasMetadataOrNull)
                    : ImgLinker.ImageOnly(url, formatSettings, Params.HasMetadataOrNull).Url;
                var source = ToSic.Razor.Blade.Tag.Source().Type(resizeFormat.MimeType).Srcset(srcSet);
                if (!string.IsNullOrEmpty(Sizes)) source.Sizes(Sizes);
                return source;
            });
        var result = ToSic.Razor.Blade.Tag.TagList(sources);
        return l.Return(result, $"{result.Count()}");
    }

}