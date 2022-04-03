using ToSic.Eav;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Images
{
    public partial class ImageService
    {
        /// <inheritdoc />
        public IResizeSettings Settings(
            object settings = default,
            string noParamOrder = Parameters.Protector,
            object factor = default,
            object width = default,
            object height = default,
            object quality = default,
            string resizeMode = default,
            string scaleMode = default,
            string format = default,
            object aspectRatio = default,
            string parameters = default,
            object recipe = default
            )
        {
            settings = GetBestSettings(settings);

            return ImgLinker.ResizeParamMerger.BuildResizeSettings(noParamOrder: noParamOrder, settings: settings, factor: factor,
                width: width, height: height, quality: quality, resizeMode: resizeMode,
                scaleMode: scaleMode, format: format, aspectRatio: aspectRatio, parameters: parameters, advanced: ToAdv(recipe));
        }


        //private ResizeSettings EnsureSettings(object settings, object factor, object recipe)
        //{
        //    var wrapLog = Log.SafeCall<ResizeSettings>(Debug);
        //    settings = GetBestSettings(settings);
        //    var resSettings = ImgLinker.ResizeParamMerger.BuildResizeSettings(settings, factor: factor, advanced: ToAdv(recipe));
        //    return wrapLog("ok", resSettings);
        //}

    }
}
