using System.Collections.Generic;
using System.Linq;
using ToSic.Eav;
using ToSic.Eav.Configuration;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;
using ToSic.Razor.Markup;
using IFeaturesService = ToSic.Sxc.Services.IFeaturesService;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Images
{
    public class ResponsivePicture: ResponsiveBase, IResponsivePicture
    {
        internal ResponsivePicture(
            ImageService imgService,
            IFeaturesService featuresService,
            string url, 
            object settings, 
            string noParamOrder = Parameters.Protector, 
            object factor = null, 
            string srcSet = null,
            string imgAlt = null,
            string imgClass = null
            ) : base(imgService, url, settings, factor: factor, srcSet: srcSet, imgAlt: imgAlt, imgClass: imgClass, logName: $"{Constants.SxcLogName}.PicSet")
        {
            _featuresService = featuresService;
        }
        private readonly IFeaturesService _featuresService;

        public Img Img
        {
            get
            {
                if (_imgTag != null)
                    return _imgTag;

                _imgTag = Tag.Img().Src(ImgLinker.Image(Url, new ResizeSettings(Settings, false), null));
                // Only add these if they were really specified
                if (ImgAlt != null) _imgTag.Alt(ImgAlt);
                if (ImgClass != null) _imgTag.Class(ImgClass);
                return _imgTag;
            }
        }

        private Img _imgTag;

        public Picture Picture => _pictureTag ?? (_pictureTag = Tag.Picture(SourceTagsInternal(Url, Settings), Img));
        private Picture _pictureTag;

        public TagList Sources => _sourceTags ?? (_sourceTags = SourceTagsInternal(Url, Settings));
        private TagList _sourceTags;

        private TagList SourceTagsInternal(string url, IResizeSettings resizeSettings)
        {
            // Check formats
            var defFormat = ImgService.GetFormat(url);
            if (defFormat == null || defFormat.ResizeFormats.Count == 0) return Tag.TagList();

            // Determine if the feature MultiFormat is enabled, if yes, use list, otherwise use only current
            var formats = _featuresService.IsEnabled(FeaturesCatalog.ImageServiceMultiFormat.NameId)
                ? defFormat.ResizeFormats
                : new List<IImageFormat> { defFormat };

            // Generate Meta Tags
            var sources = formats //  defFormat.ResizeFormats
                .Select(resizeFormat =>
                {
                    var formatSettings = new ResizeSettings(resizeSettings, true);
                    if (resizeFormat != defFormat) formatSettings.Format = resizeFormat.Format;
                    return Tag.Source().Type(resizeFormat.MimeType)
                        .Srcset(ImgLinker.Image(url, formatSettings));
                });
            var result = Tag.TagList(sources);
            return result;
        }


        public override string ToString() => Picture.ToString();
    }
}
