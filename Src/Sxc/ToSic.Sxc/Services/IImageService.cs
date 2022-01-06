using System.Web;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Images;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Services
{
    /// <summary>
    /// Service to help create responsive `img` and `picture` tags the best possible way.
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("Still WIP")]
    public interface IImageService
    {
        /// <summary>
        /// Get the format information for a specific extension.
        /// Mostly used internally, you will usually not need this. 
        /// </summary>
        /// <param name="path">Path or extension</param>
        /// <returns></returns>
        /// <remarks>Only works for the basic, known image types</remarks>
        [PrivateApi("Not sure if this is needed outside...")]
        IImageFormat GetFormat(string path);

        /// <summary>
        /// Construct custom Resize-Settings as needed, either based on existing settings or starting from scratch
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="noParamOrder"></param>
        /// <param name="factor"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="quality"></param>
        /// <param name="resizeMode"></param>
        /// <param name="scaleMode"></param>
        /// <param name="format"></param>
        /// <param name="aspectRatio"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        IResizeSettings GetResizeSettings(
            object settings = null,
            string noParamOrder = Eav.Parameters.Protector,
            object factor = null,
            object width = null,
            object height = null,
            object quality = null,
            string resizeMode = null,
            string scaleMode = null,
            string format = null,
            object aspectRatio = null,
            string parameters = null
        );

        IHybridHtmlString SrcSet(
            string url,
            object settings = null,
            string noParamOrder = Eav.Parameters.Protector,
            object factor = null,
            string srcSet = null
        );

        IResponsivePicture Picture(
            string url,
            object settings = null,
            string noParamOrder = Eav.Parameters.Protector,
            object factor = null,
            string srcSet = null,
            string imgAlt = null,
            string imgClass = null
        );

        IResponsiveImg Img(
            string url,
            object settings = null,
            string noParamOrder = Eav.Parameters.Protector,
            object factor = null,
            string srcSet = null,
            string imgAlt = null,
            string imgClass = null);
            
    }
}
