using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Razor.Html5;
using ToSic.Razor.Markup;
using static ToSic.Sxc.Configuration.Features.BuiltInFeatures;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Images
{
    public class ResponsivePicture: ResponsiveBase, IResponsivePicture
    {
        internal ResponsivePicture(ImageService imgService, ResponsiveParams callParams) : base(imgService, callParams, "Picture")
        {
        }


        public Picture Picture => _picTag.Get(() => Razor.Blade.Tag.Picture(Sources, Img));
        private readonly ValueGetOnce<Picture> _picTag = new ValueGetOnce<Picture>();

        protected override ITag GetOutermostTag() => Picture;

        public TagList Sources => _sourceTags.Get(() => SourceTagsInternal(Call.Link.Url, Settings));
        private readonly ValueGetOnce<TagList> _sourceTags = new ValueGetOnce<TagList>();

        private TagList SourceTagsInternal(string url, IResizeSettings resizeSettings)
        {
            var logOrNull = ImgService.Debug ? Log : null;
            var wrapLog = logOrNull.SafeCall<TagList>();
            // Check formats
            var defFormat = ImgService.GetFormat(url);
            if (defFormat == null) return wrapLog("no format", Razor.Blade.Tag.TagList());

            // Determine if we have many formats, otherwise just use the current one
            var formats = defFormat.ResizeFormats.Any()
                ? defFormat.ResizeFormats
                : new List<IImageFormat> { defFormat };
            
            var useMultiSrcSet = ImgService.Features.IsEnabled(ImageServiceMultipleSizes.NameId);

            Log.SafeAdd($"{nameof(formats)}: {formats.Count}, {nameof(useMultiSrcSet)}: {useMultiSrcSet}");

            // Generate Meta Tags
            var sources = formats
                .Select(resizeFormat =>
                {
                    // We must copy the settings, because we change them and this shouldn't affect anything else
                    var formatSettings = new ResizeSettings(resizeSettings, format: resizeFormat != defFormat ? resizeFormat.Format : null);
                    var srcSet = useMultiSrcSet
                        ? ImgLinker.SrcSet(url, formatSettings, SrcSetType.Source, Call.Field)
                        : ImgLinker.ImageOnly(url, formatSettings, Call.Field).Url;
                    var source = Razor.Blade.Tag.Source().Type(resizeFormat.MimeType).Srcset(srcSet);
                    if (!string.IsNullOrEmpty(Sizes)) source.Sizes(Sizes);
                    return source;
                });
            var result = Razor.Blade.Tag.TagList(sources);
            return wrapLog($"{result.Count()}", result);
        }

    }
}
