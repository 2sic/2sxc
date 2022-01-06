using System;
using ToSic.Eav.Documentation;
using ToSic.Razor.Markup;
using ToSic.Razor.Html5;
using ToSic.Sxc.Images;

namespace ToSic.Sxc.Services.Image
{
    [PrivateApi]
    public interface IImageService
    {
        /// <summary>
        /// Get the format
        /// </summary>
        /// <param name="path">Path or extension</param>
        /// <returns></returns>
        /// <remarks>Only works for the basic, known image types</remarks>
        IImageFormat GetFormat(string path);

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

        string SrcSet(
            string url,
            object settings = null,
            string noParamOrder = Eav.Parameters.Protector,
            object factor = null,
            string srcSet = null
        );


        ITag SourceTags(
            string url,
            object settings = null,
            string noParamOrder = Eav.Parameters.Protector,
            object factor = null,
            string srcSet = null
        );

        Picture PictureTag(
            string url,
            object settings = null,
            string noParamOrder = Eav.Parameters.Protector,
            object factor = null,
            string srcSet = null,
            string alt = null,
            Action<Img> imgAction = null // todo: would be Action on the img tag
        );
    }
}
