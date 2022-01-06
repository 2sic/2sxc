using System;
using System.Collections.Generic;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Images
{
    [PrivateApi("Can and will change any time, don't use outside of 2sxc")]
    public class ImageConstants
    {
        public const string Jpg = "jpg";
        public const string Gif = "gif";
        public const string Png = "png";
        public const string Svg = "svg";
        public const string Tif = "tif";
        public const string Webp = "webp";

        public static readonly Dictionary<string, ImageFormat> FileTypes = BuildFileTypes();

        /// <summary>
        /// Note: we're keeping our own list, because they are not many, and because the APIs in .net core/framework are different to find the mime types
        /// </summary>
        /// <returns></returns>
        private static Dictionary<string, ImageFormat> BuildFileTypes()
        {
            var webPInfo = new ImageFormat(Webp, "image/webp", true);
            var dic = new Dictionary<string, ImageFormat>(StringComparer.InvariantCultureIgnoreCase)
            {
                { Jpg, new ImageFormat(Jpg, "image/jpeg", true, new List<ImageFormat> { webPInfo }) },
                { Gif, new ImageFormat(Gif, "image/gif", true) },
                { Png, new ImageFormat(Png, "image/png", true, new List<ImageFormat> { webPInfo }) },
                { Svg, new ImageFormat(Svg, "image/svg+xml", false) },
                { Tif, new ImageFormat(Tif, "image/tiff", true) },
                { Webp, webPInfo }
            };
            dic["jpeg"] = dic[Jpg];
            dic["tiff"] = dic[Tif];
            return dic;
        }
    }
}
