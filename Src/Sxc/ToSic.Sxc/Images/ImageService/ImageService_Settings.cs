using ToSic.Eav;
using static ToSic.Sxc.Plumbing.ParseObject;

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
            object srcset = default,
            string advanced = default
            )
        {
            settings = GetBestSettings(settings);
            
            // If we have initial settings and srcSet isn't specified, then we should set to true so it will auto-reuse
            if (srcset == null && settings != null) 
                srcset = true;
            return ImgLinker.ResizeParamMerger.BuildResizeSettings(noParamOrder: noParamOrder, settings: settings, factor: factor,
                width: width, height: height, quality: quality, resizeMode: resizeMode,
                scaleMode: scaleMode, format: format, aspectRatio: aspectRatio, parameters: parameters, srcset: srcset/*, advanced: factorMap*/);
        }

        public IMultiResizeRule MultiResizeRule(
            string noParamOrder = Parameters.Protector,
            double factor = default,
            string srcset = default,
            string sizes = default,
            string media = default
            )
        {
            var rule = new MultiResizeRule();
            if (!DNearZero(factor)) rule.Factor = factor;
            if (srcset != default) rule.SrcSet = srcset;
            if (media != default) rule.Media = media;
            if (sizes != default) rule.Sizes = sizes;
            return rule;
        }
    }
}
