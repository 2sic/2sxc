using ToSic.Eav;

namespace ToSic.Sxc.Images
{
    public partial class ImageService
    {
        /// <inheritdoc />
        public IResizeSettings ResizeSettings(
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
            object rules = default
            )
        {
            settings = GetBestSettings(settings);
            
            // If we have initial settings and srcSet isn't specified, then we should set to true so it will auto-reuse
            //if (srcset == null && settings != null) 
            //    srcset = true;
            var allowMulti = (rules == null && settings != null);
            return ImgLinker.ResizeParamMerger.BuildResizeSettings(noParamOrder: noParamOrder, settings: settings, factor: factor,
                width: width, height: height, quality: quality, resizeMode: resizeMode,
                scaleMode: scaleMode, format: format, aspectRatio: aspectRatio, parameters: parameters, allowMulti: allowMulti, advanced: rules);
        }

    }
}
