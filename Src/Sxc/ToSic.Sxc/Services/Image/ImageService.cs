using System;
using System.Linq;
using ToSic.Eav;
using ToSic.Eav.Logging;
using ToSic.Razor.Blade;
using ToSic.Razor.Html5;
using ToSic.Razor.Markup;
using ToSic.Sxc.Images;

namespace ToSic.Sxc.Services.Image
{
    public partial class ImageService: HasLog<ImageService>, IImageService
    {
        public ImageService(ImgResizeLinker imgLinker) : base(Constants.SxcLogName + ".ImgSvc") => ImgLinker = imgLinker.Init(Log);
        protected ImgResizeLinker ImgLinker { get; }

        // todo
        // - get SrcSet into settings as well if possible

        public ITag SourceTags(string url, object settings = null, string noParamOrder = Parameters.Protector, object factor = null, string srcSet = null)
        {
            var resizeSettings = PrepareResizeSettings(settings, factor);

            return SourceTagsInternal(url, resizeSettings, srcSet);
        }

        private ITag SourceTagsInternal(string url, IResizeSettings resizeSettings, string srcSet)
        {
            // Check formats
            var defFormat = GetFormat(url);
            if (defFormat == null || defFormat.ResizeFormats.Count == 0) return Tag.Custom(null);

            // Generate Meta Tags
            var sources = defFormat.ResizeFormats
                .Select(resizeFormat =>
                {
                    var formatSettings = new ResizeSettings(resizeSettings);
                    if (resizeFormat != defFormat) formatSettings.Format = resizeFormat.Format;
                    return Tag.Source().Type(resizeFormat.MimeType)
                            .Srcset(ImgLinker.ImageOrSrcSetLink(url, formatSettings, srcSet));
                });
            var result = Tag.Custom(null, sources);
            return result;
        }

        public Picture PictureTag(string url, object settings = null,
            string noParamOrder =
                "Rule: all params must be named (https://r.2sxc.org/named-params), Example: \'enable: true, version: 10\'",
            object factor = null, string srcSet = null,
            string alt = null, Action<Img> imgAction = null)
        {
            var resizeSettings = PrepareResizeSettings(settings, factor);

            // Create the img tag
            var imgTag = Tag.Img().Src(ImgLinker.ImageOrSrcSetLink(url, resizeSettings, null));

            try
            {
                imgAction?.Invoke(imgTag);
            }
            catch (Exception ex)
            {
                Log.Add("Error invoking custom img-tag action");
                Log.Exception(ex);
            }

            // Create the picture tag to return later
            var picTag = Tag.Picture(
                SourceTagsInternal(url, resizeSettings, srcSet),
                imgTag
            );


            return picTag;
        }
        
        private IResizeSettings PrepareResizeSettings(object settings, object factor)
        {
            // 1. Prepare Settings
            if (!(settings is IResizeSettings resizeSettings))
                resizeSettings = ImgLinker.ResizeParamMerger.BuildResizeParameters(settings, factor: factor);
            return resizeSettings;
        }

    }
}
