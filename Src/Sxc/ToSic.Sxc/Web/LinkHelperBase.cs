using System;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Razor.Blade;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;
using ToSic.Sxc.Context;
using ToSic.Sxc.Images;
using ToSic.Sxc.Run;

namespace ToSic.Sxc.Web
{
    [PrivateApi]
    public abstract class LinkHelperBase : HasLog, ILinkHelper
    {
        protected LinkHelperBase(ImgResizeLinker imgLinker, Lazy<ILinkPaths> linkPathsLazy) : base($"{Constants.SxcLogName}.LnkHlp")
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
            // prevent incorrect use without named parameters
            Eav.Parameters.ProtectAgainstMissingParameterNames(noParamOrder, $"{nameof(To)}", $"{nameof(pageId)},{nameof(parameters)},{nameof(api)}");

            // Check initial conflicting values.
            if (pageId != null && api != null)
                throw new ArgumentException($"Only one of the parameters '{nameof(api)}' or '{nameof(pageId)}' can have a value.");

            var strParams = ParametersToString(parameters);

            // TODO: unclear what would happen if a new parameter would replace an existing - would it just append? that wouldn't be good
            var url = api == null
                ? ToPage(pageId, strParams, language)
                : ToApi(api, strParams);

            var processed = ExpandUrlIfNecessary(type, url);

            return Tags.SafeUrl(processed).ToString();
        }

        private string ExpandUrlIfNecessary(string type, string url)
        {
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
        public string Image(string url = null,
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
            string type = null,
            object parameters = null
            )
        {
            // prevent incorrect use without named parameters
            Eav.Parameters.ProtectAgainstMissingParameterNames(noParamOrder, $"{nameof(Image)}", $"{nameof(width)},{nameof(height)}," +
                $"{nameof(quality)},{nameof(resizeMode)},{nameof(scaleMode)},{nameof(format)},{nameof(aspectRatio)},{nameof(type)},{nameof(parameters)}");

            // If params were given, ensure it can be used as string, as it could also be a params-object
            var strParams = ParametersToString(parameters);

            // If the url should be expanded to have a full root or something, do this first
            var expandedUrl = ExpandUrlIfNecessary(type, url);

            // Get the image-url(s) as needed
            var imageUrl = ImgLinker.Image(expandedUrl, settings, factor, width: width, height: height, quality: quality, resizeMode: resizeMode,
                scaleMode: scaleMode, format: format, aspectRatio: aspectRatio, parameters: strParams, srcSet: false);

            return imageUrl;
        }

        private bool _debug;
        public void SetDebug(bool debug)
        {
            _debug = debug;
            // Set logging on ImageResizeHelper
            ImgLinker.Debug = debug;
        }
        
        public abstract string GetCurrentLinkRoot();

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
    }
}