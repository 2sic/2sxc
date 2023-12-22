using ToSic.Lib.Coding;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Tests.LinksAndImages.LinkHelperTests
{
    public static class LinkTestHelperExtensions
    {
        /// <summary>
        /// Special helper to avoid accessing the real To so many times
        /// </summary>
        public static string TestTo(this ILinkService link,
            NoParamOrder noParamOrder = default,
            int? pageId = null,
            object parameters = null,
            string api = null,
            string type = null)
        {
            return link.To(noParamOrder: noParamOrder, pageId: pageId, parameters: parameters, api: api, type: type);
        }


        public static string TestImage(this ILinkService link,
            string url = null,
            object settings = null,
            object factor = null,
            NoParamOrder noParamOrder = default,
            object width = null,
            object height = null,
            object quality = null,
            string resizeMode = null,
            string scaleMode = null,
            string format = null,
            object aspectRatio = null,
            string type = null
            )
            => link.Image(url, settings, factor, noParamOrder, width: width, height: height, quality: quality, resizeMode: resizeMode, scaleMode: scaleMode, format: format,
                aspectRatio: aspectRatio, type: type);
    }
}
