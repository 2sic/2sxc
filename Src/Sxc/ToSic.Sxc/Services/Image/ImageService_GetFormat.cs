using System.IO;
using ToSic.Sxc.Images;

namespace ToSic.Sxc.Services.Image
{
    public partial class ImageService
    {
        /// <inheritdoc />
        public IImageFormat GetFormat(string path)
        {
            // 1. check extension makes sense / lower case
            if (string.IsNullOrWhiteSpace(path)) return new ImageFormat("", "", false);
            path = path.Split('?')[0];
            var extension = path.ToLowerInvariant();
            if (extension.Contains(".")) extension = Path.GetExtension(extension).Trim('.');

            // 2. See if we know of this - if yes, return
            if (ImageConstants.FileTypes.TryGetValue(extension, out var result)) return result;

            // 3. Otherwise just return an object without known mime type
            return new ImageFormat(extension, "", false);

            // 4. Future / maybe: Otherwise check system for mime type and try to build a recommendation
        }

        public IResizeSettings GetResizeSettings(
            object settings = null,
            object factor = null,
            string noParamOrder = Eav.Parameters.Protector,
            object width = null,
            object height = null,
            object quality = null,
            string resizeMode = null,
            string scaleMode = null,
            string format = null,
            object aspectRatio = null,
            string parameters = null
            )
        {
            return ImgLinker.ResizeParamMerger.BuildResizeParameters(settings: settings, factor: factor,
                noParamOrder: noParamOrder, width: width, height: height, quality: quality, resizeMode: resizeMode,
                scaleMode: scaleMode, format: format, aspectRatio: aspectRatio, parameters: parameters);
        }
    }
}
