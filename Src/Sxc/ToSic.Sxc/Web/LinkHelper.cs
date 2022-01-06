using System;
using ToSic.Eav.Documentation;
using ToSic.Eav.Helpers;
using ToSic.Eav.Logging;
using ToSic.Razor.Blade;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;
using ToSic.Sxc.Context;
using ToSic.Sxc.Images;

namespace ToSic.Sxc.Web
{
    [PrivateApi]
    public abstract class LinkHelper : HasLog, ILinkHelper
    {
        private ImgResizeLinker ImgLinker { get; }
        [PrivateApi] protected IApp App;

        protected LinkHelper(ImgResizeLinker imgLinker) : base($"{Constants.SxcLogName}.LnkHlp")
        {
            ImgLinker = imgLinker;
            ImgLinker.Init(Log);
        }

        public virtual void AddBlockContext(IDynamicCodeRoot codeRoot)
        {
            Log.LinkTo(codeRoot.Log);
            App = codeRoot.App;
        }


        /// <inheritdoc />
        public string To(
            string noParamOrder =
                "Rule: all params must be named (https://r.2sxc.org/named-params), Example: \'enable: true, version: 10\'",
            int? pageId = null,
            string api = null,
            object parameters = null,
            string type = null)
        {
            // prevent incorrect use without named parameters
            Eav.Parameters.ProtectAgainstMissingParameterNames(noParamOrder, $"{nameof(To)}", $"{nameof(pageId)},{nameof(parameters)},{nameof(api)}");

            // Check initial conflicting values.
            if (pageId != null && api != null)
                throw new ArgumentException($"Only one of the parameters '{nameof(api)}' or '{nameof(pageId)}' can have a value.");

            var strParams = ParametersToString(parameters);

            // TODO: unclear what would happen if a new parameter would replace an existing - would it just append? that wouldn't be good
            var url = api == null
                ? ToPage(pageId, strParams)
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
                        parts.ReplaceRoot(GetCurrentRequestUrl());
                    return parts.ToLink("full");
                case "//":
                    if (!parts.IsAbsolute)
                        parts.ReplaceRoot(GetCurrentRequestUrl());
                    return parts.ToLink("//");
                case "/": // note: "/" isn't officially supported
                    return parts.ToLink("/");
                default:
                    return url;
            }
        }
        

        protected abstract string ToApi(string api, string parameters = null);

        protected abstract string ToPage(int? pageId, string parameters = null);

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
            string noParamOrder =
                "Rule: all params must be named (https://r.2sxc.org/named-params), Example: \'enable: true, version: 10\'",
            object width = null,
            object height = null,
            object quality = null,
            string resizeMode = null,
            string scaleMode = null,
            string format = null,
            object aspectRatio = null,
            string type = null,
            object parameters = null, 
            string srcSet = null
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
                scaleMode: scaleMode, format: format, aspectRatio: aspectRatio, parameters: strParams, srcSet: srcSet);

            return imageUrl;
        }

        private bool _debug;
        public void SetDebug(bool debug)
        {
            _debug = debug;
            // Set logging on ImageResizeHelper
            ImgLinker.Debug = debug;
        }
        

        private static bool IsInvalidUrl(UrlParts parts)
        {
            // if the url seems invalid (like `hello:there` or an invalid `file:593902` reference) nothing is added
            if (parts.BuildUrl().Contains(":"))
                return true;

            // if the url starts with `../` (like `../image.jpg`) than nothing to do
            if (parts.Path.StartsWith("../"))
                return true;

            // if the url has with `/../` (like `/sibling1/../sibling2/image.jpg`) than nothing to do
            if (parts.Path.Contains("/../"))
                return true;

            //var converter = new UriTypeConverter();
            //if (!converter.IsValid(parts.Path))
            //    return true;

            //if (!Uri.IsWellFormedUriString(parts.Path, UriKind.Relative))
            //    return true;

            //if (!string.IsNullOrWhiteSpace(parts.Path.TrimStart('/')) && Uri.TryCreate(parts.Path.TrimStart('/'), UriKind.RelativeOrAbsolute, out Uri uriResult))
            //    return true;

            return false;
        }

        // TODO: review w/Tonci STV if this is necessary any where
        private string UrlIsRelative(UrlParts parts)
        {
            // clean "~" from path
            if (parts.Path.StartsWith("~"))
                parts.Path = parts.Path.TrimStart('~').PrefixSlash(); // ensure that we get host instead of current page url

            // invalid urls
            if (IsInvalidUrl(parts)) return parts.Url;

            // create absolute url
            parts.Path = $"{GetCurrentLinkRoot()}{parts.Path.PrefixSlash()}";

            return parts.BuildUrl();
        }

        public abstract string GetCurrentLinkRoot();

        public abstract string GetCurrentRequestUrl();
        
    }
}