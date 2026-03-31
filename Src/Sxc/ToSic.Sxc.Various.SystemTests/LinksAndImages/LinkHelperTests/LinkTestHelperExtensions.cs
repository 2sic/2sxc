using ToSic.Sxc.Services;

namespace ToSic.Sxc.LinksAndImages.LinkHelperTests;

public static class LinkTestHelperExtensions
{
    extension(ILinkService link)
    {
        /// <summary>
        /// Special helper to avoid accessing the real To so many times
        /// </summary>
        public string ToTac(NoParamOrder npo = default,
            int? pageId = null,
            object? parameters = null,
            string? api = null,
            string? type = null) =>
            link.To(npo: npo, pageId: pageId, parameters: parameters, api: api, type: type);

        public string? ImageTac(string? url = null,
            object? settings = null,
            object? factor = null,
            NoParamOrder npo = default,
            object? width = null,
            object? height = null,
            object? quality = null,
            string? resizeMode = null,
            string? scaleMode = null,
            string? format = null,
            object? aspectRatio = null,
            string? type = null,
            string? parameters = null) =>
            link.Image(url, settings, factor, npo, width: width, height: height, quality: quality, resizeMode: resizeMode, scaleMode: scaleMode, format: format,
                aspectRatio: aspectRatio, type: type, parameters: parameters);
    }
}