using ToSic.Eav;
using ToSic.Eav.Logging;
using ToSic.Sxc.Plumbing;

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


        private ResizeSettings EnsureSettings(object settings, object factor, object recipe)
        {
            var wrapLog = Log.SafeCall<ResizeSettings>(Debug);
            var advanced = ToAdv(recipe);
            // 1. Prepare Settings
            if (settings is ResizeSettings resSettings)
            {
                // If we have a modified factor, make sure we have that (this will copy the settings)
                var newFactor = ParseObject.DoubleOrNullWithCalculation(factor);
                Log.SafeAdd(Debug, $"Is {nameof(ResizeSettings)}, now with new factor: {newFactor}, will clone/init");
                resSettings = new ResizeSettings(resSettings, factor: newFactor ?? resSettings.Factor, advanced: advanced);
            }
            else
            {
                Log.SafeAdd(Debug, $"Not {nameof(ResizeSettings)}, will create");
                resSettings = ImgLinker.ResizeParamMerger.BuildResizeSettings(settings, factor: factor, advanced: advanced);
            }

            return wrapLog("ok", resSettings);
        }

    }
}
