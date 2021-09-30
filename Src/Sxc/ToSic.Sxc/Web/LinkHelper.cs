using System;
using System.Web;
using ToSic.Eav.Documentation;
using ToSic.Eav.Helpers;
using ToSic.Eav.Logging;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;
using ToSic.Sxc.Context;

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
        public string To(string noParamOrder = Eav.Parameters.Protector, int? pageId = null, object parameters = null, string api = null)
        {
            // prevent incorrect use without named parameters
            Eav.Parameters.ProtectAgainstMissingParameterNames(noParamOrder, $"{nameof(To)}", $"{nameof(pageId)},{nameof(parameters)},{nameof(api)}");

            // Check initial conflicting values.
            if (pageId != null && api != null)
                throw new ArgumentException($"Multiple properties like '{nameof(api)}' or '{nameof(pageId)}' have a value - only one can be provided.");

            var strParams = ParametersToString(parameters);

            return ToImplementation(pageId, strParams, api);
        }

        protected abstract string ToImplementation(int? pageId = null, string parameters = null, string api = null);

        protected string ParametersToString(object parameters)
        {
            if (parameters is null) return null;
            if (parameters is string strParameters) return strParameters;
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
            string part = null)
        {
            var relativeOrAbsoluteUrl = (part == "full") ? FullUrl(url) : url;

            return ImgLinker.Image(url: relativeOrAbsoluteUrl, settings, factor, noParamOrder, width, height, quality, resizeMode,
                scaleMode, format, aspectRatio);
        }

        private bool _debug;
        public void SetDebug(bool debug)
        {
            _debug = debug;
            // Set logging on ImageResizeHelper
            ImgLinker.Debug = debug;
        }

        protected string FullUrl(string url)
        {
            var parts = new UrlParts(url);

            // no path or just query string
            if (string.IsNullOrEmpty(parts.Path))
            {
                return UrlPathIsMissing(parts);
            }

            // absolute url already provided
            if (IsAbsoluteUrl(parts))
            {
                return UrlIsAbsolute(parts);
            }

            // relative urls
            return UrlIsRelative(parts);
        }

        // when no url or just query params was provided would just result in the domain + link to the current page as is
        private string UrlPathIsMissing(UrlParts parts)
        {
            var currentRequestParts = new UrlParts(GetCurrentRequestUrl());

            // handle fragments
            if (!string.IsNullOrEmpty(parts.Fragment))
                currentRequestParts.Fragment = parts.Fragment;

            // handle query strings
            if (!string.IsNullOrEmpty(parts.Query))
            {
                if (string.IsNullOrEmpty(currentRequestParts.Query))
                {
                    currentRequestParts.Query = parts.Query;
                }
                else
                {
                    // combine query strings
                    var queryString = HttpUtility.ParseQueryString(parts.Query);
                    var currentRequestQueryString = HttpUtility.ParseQueryString(currentRequestParts.Query);
                    foreach (var key in queryString.AllKeys)
                    {
                        currentRequestQueryString.Set(key, queryString.Get(key));
                    }

                    currentRequestParts.Query = currentRequestQueryString.ToString();
                }
            }

            return currentRequestParts.BuildUrl();
        }

        private static bool IsAbsoluteUrl(UrlParts parts)
        {
            return parts.Path.StartsWith("//") || parts.Path.StartsWith("http://") || parts.Path.StartsWith("https://");
        }

        private string UrlIsAbsolute(UrlParts parts)
        {
            // if a url is provided without protocol, it's assumed that it's on the current site, so the current domain/protocol are added
            if (parts.Path.StartsWith("//"))
            {
                var protocol = (new Uri(GetDomainName(), UriKind.Absolute)).Scheme;
                parts.Path = $"{protocol}:{parts.Path}";
            }

            return parts.BuildUrl();
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

        private string UrlIsRelative(UrlParts parts)
        {
            // clean "~" from path
            if (parts.Path.StartsWith("~"))
                parts.Path = parts.Path.TrimStart('~').PrefixSlash(); // ensure that we get host instead of current page url

            // invalid urls
            if (IsInvalidUrl(parts))
            {
                return parts.Url;
            }

            // create absolute url
            parts.Path = $"{GetDomainName()}{parts.Path.PrefixSlash()}";

            return parts.BuildUrl();
        }

        public abstract string GetDomainName();

        public abstract string GetCurrentRequestUrl();
    }
}