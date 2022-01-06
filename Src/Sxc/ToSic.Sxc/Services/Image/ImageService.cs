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
            var resizeSettings = PrepareResizeSettings(settings, factor, srcSet);
            return SourceTagsInternal(url, resizeSettings);
        }

        private ITag SourceTagsInternal(string url, IResizeSettings resizeSettings)
        {
            // Check formats
            var defFormat = GetFormat(url);
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

        public Picture PictureTag(
            string url, 
            object settings = null,
            string noParamOrder = Parameters.Protector,
            object factor = null, 
            string srcSet = null,
            string alt = null, 
            Action<Img> imgAction = null)
        {
            var resizeSettings = PrepareResizeSettings(settings, factor, srcSet);

            // Create the img tag
            var imgTag = Tag.Img().Src(ImgLinker.Image(url, new ResizeSettings(resizeSettings, false), null));

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
                SourceTagsInternal(url, resizeSettings),
                imgTag
            );


            return picTag;
        }
        
        private IResizeSettings PrepareResizeSettings(object settings, object factor, string srcSet)
        {
            // 1. Prepare Settings
            if (!(settings is IResizeSettings resizeSettings))
            {
                resizeSettings = ImgLinker.ResizeParamMerger.BuildResizeParameters(settings, factor: factor);
            }
            else
            {
                // TODO: STILL USE THE FACTOR!
            }

            if (srcSet != null) resizeSettings.SrcSet = srcSet;

            return resizeSettings;
        }

    }
}
