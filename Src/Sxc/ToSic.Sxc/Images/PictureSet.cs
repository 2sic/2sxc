using System.Linq;
using ToSic.Eav;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;
using ToSic.Razor.Markup;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Images
{
    public class PictureSet: ImgPicSetBase, IPictureSet
    {
        internal PictureSet(ImageService imgService, string url, object settings, string noParamOrder = Parameters.Protector, object factor = null, string srcSet = null)
            : base(imgService, url, settings, factor: factor, srcSet: srcSet, logName: $"{Constants.SxcLogName}.PicSet")
        {
        }

        public Img ImgTag => _imgTag ?? (_imgTag = Tag.Img().Src(ImgLinker.Image(Url, new ResizeSettings(Settings, false), null)));
        private Img _imgTag;

        public Picture PictureTag => _pictureTag ?? (_pictureTag = Tag.Picture(SourceTagsInternal(Url, Settings), ImgTag));
        private Picture _pictureTag;

        public ITag SourceTags => _sourceTags ?? (_sourceTags = SourceTagsInternal(Url, Settings));
        private ITag _sourceTags;

        private ITag SourceTagsInternal(string url, IResizeSettings resizeSettings)
        {
            // Check formats
            var defFormat = ImgService.GetFormat(url);
            if (defFormat == null || defFormat.ResizeFormats.Count == 0) return Tag.Custom(null);

            // Generate Meta Tags
            var sources = defFormat.ResizeFormats
                .Select(resizeFormat =>
                {
                    var formatSettings = new ResizeSettings(resizeSettings, true);
                    if (resizeFormat != defFormat) formatSettings.Format = resizeFormat.Format;
                    return Tag.Source().Type(resizeFormat.MimeType)
                        .Srcset(ImgLinker.Image(url, formatSettings));
                });
            var result = Tag.Custom(null, sources);
            return result;
        }


        public override string ToString() => PictureTag.ToString();
    }
}
