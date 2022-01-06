using System;
using ToSic.Eav.Documentation;
using ToSic.Razor.Markup;
using ToSic.Sxc.Images;

namespace ToSic.Sxc.Services.Image
{
    [PrivateApi]
    public interface IImageService
    {
        /// <summary>
        /// Get a format information for the specified extension or image. Will help you create the appropriate resize/image tags.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        IImageFormat GetFormat(string path);


        ITag SourceTags(
            string url,
            object settings = null,
            string noParamOrder = Eav.Parameters.Protector,
            object formats = null,
            object factor = null
        );

        object PictureTag(
            string url,
            object settings = null,
            string noParamOrder = Eav.Parameters.Protector,
            object factor = null,
            string alt = null,
            Action<string> img = null // todo: would be Action on the img tag
        );
    }
}
