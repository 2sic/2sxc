using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Configuration;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;
using ToSic.Razor.Markup;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Images
{
    public class ResponsivePicture: ResponsiveBase, IResponsivePicture
    {
        internal ResponsivePicture(ImageService imgService, ResponsiveParams responsiveParams) : base(imgService, responsiveParams)
        {
        }


        public Picture Picture => _pictureTag ?? (_pictureTag = Tag.Picture(Sources, Img));
        private Picture _pictureTag;

        public TagList Sources => _sourceTags ?? (_sourceTags = SourceTagsInternal(Call.Link.Url, Settings));
        private TagList _sourceTags;

        private TagList SourceTagsInternal(string url, IResizeSettings resizeSettings)
        {
            // Check formats
            var defFormat = ImgService.GetFormat(url);
            if (defFormat == null) return Tag.TagList();

            // Determine if we have many formats, otherwise just use the current one
            var formats = defFormat.ResizeFormats.Any()
                ? defFormat.ResizeFormats
                : new List<IImageFormat> { defFormat };
            
            var useMultiSrcSet = ImgService.Features.IsEnabled(FeaturesCatalog.ImageServiceMultipleSizes.NameId);

            // Generate Meta Tags
            var sources = formats
                .Select(resizeFormat =>
                {
                    // We must copy the settings, because we change them and this shouldn't affect anything else
                    var formatSettings = new ResizeSettings(resizeSettings, format: resizeFormat != defFormat ? resizeFormat.Format : null);
                    var srcSet = useMultiSrcSet
                        ? ImgLinker.SrcSet(url, formatSettings, SrcSetType.Source, Call.Field)
                        : ImgLinker.ImageOnly(url, formatSettings, Call.Field).Url;
                    return Tag.Source().Type(resizeFormat.MimeType).Srcset(srcSet);
                });
            var result = Tag.TagList(sources);
            return result;
        }


        public override string ToString() => Picture.ToString();
    }
}
