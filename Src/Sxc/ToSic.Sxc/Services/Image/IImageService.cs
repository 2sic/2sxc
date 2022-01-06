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
        /// Get the format
        /// </summary>
        /// <param name="path">Path or extension</param>
        /// <returns></returns>
        /// <remarks>Only works for the basic, known image types</remarks>
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
