using System;
using ToSic.Eav.Documentation;
using ToSic.Lib.Logging;
using ToSic.Razor.Blade;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.Images;
using ToSic.Sxc.Run;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Services
{
    [PrivateApi]
    public abstract class LinkServiceBase : HasLog, ILinkService
    {
        protected LinkServiceBase(ImgResizeLinker imgLinker, Lazy<ILinkPaths> linkPathsLazy) : base($"{Constants.SxcLogName}.LnkHlp")
        {
            _linkPathsLazy = linkPathsLazy;
            ImgLinker = imgLinker;
            ImgLinker.Init(Log);
        }
        private ImgResizeLinker ImgLinker { get; }
        private readonly Lazy<ILinkPaths> _linkPathsLazy;
        public ILinkPaths LinkPaths => _linkPathsLazy.Value;

        public virtual void ConnectToRoot(IDynamicCodeRoot codeRoot)
        {
            CodeRoot = codeRoot;
            Log.LinkTo(codeRoot.Log);
        }
        [PrivateApi] protected IDynamicCodeRoot CodeRoot;
        [PrivateApi] protected IApp App => CodeRoot.App;


        /// <inheritdoc />
        public string To(
            string noParamOrder = Eav.Parameters.Protector,
            int? pageId = null,
            string api = null,
            object parameters = null,
            string type = null,
            string language = null
            )
        {
            var wrapLog = Log.Fn<string>($"pid:{pageId},api:{api},t:{type},l:{language}");

            // prevent incorrect use without named parameters
            Eav.Parameters.ProtectAgainstMissingParameterNames(noParamOrder, $"{nameof(To)}", $"{nameof(pageId)},{nameof(parameters)},{nameof(api)}");

            // Check initial conflicting values.
            if (pageId != null && api != null)
                throw new ArgumentException($"Only one of the parameters '{nameof(api)}' or '{nameof(pageId)}' can have a value.");

            var strParams = ParametersToString(parameters);
            Log.A($"parameters:{strParams}");
 
            // TODO: unclear what would happen if a new parameter would replace an existing - would it just append? that wouldn't be good
            var url = api == null
                ? ToPage(pageId, strParams, language)
                : ToApi(api, strParams);
            Log.A($"url:{url}");

            var processed = ExpandUrlIfNecessary(type, url);
            Log.A($"expandUrl:{processed}, t:{type}");

            return wrapLog.ReturnAsOk(Tags.SafeUrl(processed).ToString());
        }

        private string ExpandUrlIfNecessary(string type, string url)
        {
            if (url == null) return null;

            // Short-Circuit to really not do anything if the type isn't specified
            if (string.IsNullOrEmpty(type)) return url;

            var parts = new UrlParts(url);
            switch (type?.ToLowerInvariant())
            {
                case "full":
                    if (!parts.IsAbsolute)
                        parts.ReplaceRoot(LinkPaths.GetCurrentRequestUrl());
                    return parts.ToLink("full");
                case "//":
                    if (!parts.IsAbsolute)
                        parts.ReplaceRoot(LinkPaths.GetCurrentRequestUrl());
                    return parts.ToLink("//");
                case "/": // note: "/" isn't officially supported
                    return parts.ToLink("/");
                default:
                    return url;
            }
        }
     

        protected abstract string ToApi(string api, string parameters = null);

        protected abstract string ToPage(int? pageId, string parameters = null, string language = null);

        protected static string ParametersToString(object parameters)
        {
            if (parameters is null) return null;
            if (parameters is string strParameters)
                return strParameters.TrimStart('?').TrimStart('&');    // make sure leading ? and '&' are removed
            if (parameters is IParameters paramDic) return paramDic.ToString();

            // Fallback / default
            return null;
        }


        /// <inheritdoc />
        public virtual string Base()
        {
            // helper to generate a base path which is also valid on home (special DNN behaviour)
            const string randomxyz = "this-should-never-exist-in-the-url";
            var basePath = To(parameters: randomxyz + "=1");
            return basePath.Substring(0, basePath.IndexOf(randomxyz, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <inheritdoc />
        public string Image(
            string url = default,
            object settings = default,
            object factor = default,
            string noParamOrder = Eav.Parameters.Protector,
            IDynamicField field = default,
            object width = default,
            object height = default,
            object quality = default,
            string resizeMode = default,
            string scaleMode = default,
            string format = default,
            object aspectRatio = default,
            string type = default,
            object parameters = default
            )
        {
            // prevent incorrect use without named parameters
            Eav.Parameters.ProtectAgainstMissingParameterNames(noParamOrder, $"{nameof(Image)}", $"{nameof(width)},{nameof(height)}," +
                $"{nameof(quality)},{nameof(resizeMode)},{nameof(scaleMode)},{nameof(format)},{nameof(aspectRatio)},{nameof(type)},{nameof(parameters)}");

            // If params were given, ensure it can be used as string, as it could also be a params-object
            var strParams = ParametersToString(parameters);

            // If the url should be expanded to have a full root or something, do this first
            url = url ?? field?.Parent.Get(field.Name) as string;
            var expandedUrl = ExpandUrlIfNecessary(type, url);

            // Get the image-url(s) as needed
            // Note that srcset is false, so it won't generate a bunch of sources, just one - which is how the API works
            // Anybody that wants a srcset must use the new IImageService for that
            var imageUrl = ImgLinker.Image(url: expandedUrl, settings: settings, field: field, factor: factor, width: width, height: height, quality: quality, resizeMode: resizeMode,
                scaleMode: scaleMode, format: format, aspectRatio: aspectRatio, parameters: strParams);

            return imageUrl;
        }

        /// <inheritdoc />
        public bool Debug
        {
            get => _debug;
            set
            {
                _debug = value;
                ImgLinker.Debug = value;
            }
        }
        private bool _debug;

        /**
         * Combine api with query string.
         */
        public static string CombineApiWithQueryString(string api, string queryString)
        {
            queryString = queryString?.TrimStart('?').TrimStart('&');

            // combine api with query string
            return string.IsNullOrEmpty(queryString) ? api :
                api?.IndexOf("?") > 0 ? $"{api}&{queryString}" : $"{api}?{queryString}";
        }

        internal static string CurrentPageUrlWithEventualHashError(int? pageId, string currentPageUrl) 
            => !pageId.HasValue ? currentPageUrl : $"{currentPageUrl}#error-unknown-pageid-{pageId}";
    }
}