using System;
using ToSic.Eav.Documentation;
using ToSic.Eav.Helpers;
using ToSic.Eav.Logging;
using ToSic.Razor.Blade;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;
using ToSic.Sxc.Context;
using ToSic.Sxc.Web.Url;

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
        public string To(string noParamOrder = Eav.Parameters.Protector, 
            int? pageId = null, 
            object parameters = null, 
            string api = null,
            string type = null // WIP, probably "full", "root", "https", "//", "http" etc.
            /*string part = null*/)
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

            var processed = ChangeToMatchType(type, url);

            return Tags.SafeUrl(processed).ToString();
        }

        private string ChangeToMatchType(string type, string url)
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
                    // return FullUrl(url);
                case "//":
                    if (!parts.IsAbsolute)
                        parts.ReplaceRoot(GetCurrentRequestUrl());
                    return parts.ToLink("//");
                    //return Protocol(url);
                case "/": // note: "/" isn't officially supported
                    return parts.ToLink("/");
                    //return Domain(url);
                default:
                    return url;
            }
        }

        //private string ProcessPartParam(string part, string url)
        //{
        //    switch (part)
        //    {
        //        case "full":
        //            return FullUrl(url);
        //        case "protocol":
        //            return Protocol(url);
        //        case "domain":
        //            return Domain(url);
        //        case "hash":
        //            return Hash(url);
        //        case "query":
        //            return Query(url);
        //        case "suffix":
        //            return Suffix(url);
        //        default:
        //            return url;
        //    }
        //}

        protected abstract string ToApi(string api, string parameters = null);

        protected abstract string ToPage(int? pageId, string parameters = null);

        //protected abstract string ToImplementation(int? pageId = null, string parameters = null, string api = null);

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

            /*string part = null*/)
        {
            // prevent incorrect use without named parameters
            Eav.Parameters.ProtectAgainstMissingParameterNames(noParamOrder, $"{nameof(Image)}", $"{nameof(width)},{nameof(height)}," +
                $"{nameof(quality)},{nameof(resizeMode)},{nameof(scaleMode)},{nameof(format)},{nameof(aspectRatio)},{nameof(type)},{nameof(parameters)}");

            var imageUrl = ImgLinker.Image(url: url, settings, factor, noParamOrder, width, height, quality, resizeMode,
                scaleMode, format, aspectRatio);

            var strParams = ParametersToString(parameters);

            if (!string.IsNullOrWhiteSpace(strParams))
            {
                var paramList = UrlHelpers.ParseQueryString(strParams);
                if (paramList != null & paramList.HasKeys()) 
                    imageUrl = UrlHelpers.AddQueryString(imageUrl, paramList);
            }

            var processed = ChangeToMatchType(type, imageUrl);
            //var processed = ProcessPartParam(part, imageUrl);
            return Tags.SafeUrl(processed).ToString();
        }

        private bool _debug;
        public void SetDebug(bool debug)
        {
            _debug = debug;
            // Set logging on ImageResizeHelper
            ImgLinker.Debug = debug;
        }

        //protected string FullUrl(string url)
        //{
        //    var parts = new UrlParts(url);

        //    // no path or just query string
        //    if (string.IsNullOrEmpty(parts.Path))
        //    {
        //        return AddDomainAndProtocol(parts);
        //    }

        //    // absolute url already provided
        //    if (parts.IsAbsolute)
        //    {
        //        return AddCurrentProtocol(parts);
        //    }

        //    // relative urls
        //    return UrlIsRelative(parts);
        //}

        //// when no url or just query params was provided would just result in the domain + link to the current page as is
        //private string AddDomainAndProtocol(UrlParts parts)
        //{
        //    parts.ReplaceRoot(GetCurrentRequestUrl());
        //    // var currentRequestParts = new UrlParts(GetCurrentRequestUrl());
            

        //    // handle fragments
        //    //if (!string.IsNullOrEmpty(parts.Fragment))
        //    //    currentRequestParts.Fragment = parts.Fragment;

        //    // handle query strings
        //    return parts.ToLink("full"); // QueryHelper.Combine(currentRequestParts.BuildUrl(), parts.Query);
        //}


        //private string AddCurrentProtocol(UrlParts parts)
        //{
        //    // if a url is provided without protocol, it's assumed that it's on the current site, so the current domain/protocol are added
        //    if (parts.Path.StartsWith("//"))
        //    {
        //        var protocol = (new Uri(GetDomainName(), UriKind.Absolute)).Scheme;
        //        parts.Path = $"{protocol}:{parts.Path}";
        //    }

        //    return parts.BuildUrl();
        //}

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
            if (IsInvalidUrl(parts))
            {
                return parts.Url;
            }

            // create absolute url
            parts.Path = $"{GetCurrentLinkRoot()}{parts.Path.PrefixSlash()}";

            return parts.BuildUrl();
        }

        public abstract string GetCurrentLinkRoot();

        public abstract string GetCurrentRequestUrl();

        ///// <summary>
        ///// `protocol` would just return the "http", "https" or whatever.
        /////     - if no url was provided, it will assume that the current page is to be used
        /////     - if a url was provided and it has no protocol, then the current protocol is used
        /////     - if a url was provided with protocol, it would return that
        ///// </summary>
        ///// <param name="url"></param>
        ///// <returns></returns>
        //protected string Protocol(string url)
        //{
        //    return (new Uri(FullUrl(url), UriKind.Absolute)).Scheme;
        //}

        ///// <summary>
        ///// `domain` would just return the full domain like `2sxc.org`, `www.2sxc.org` or `gettingstarted.2sxc.org`
        /////     - if no url was provided, then the domain of the current page
        /////     - if the url contains a domain, then that domain
        ///// </summary>
        ///// <param name="url"></param>
        ///// <returns></returns>
        //protected string Domain(string url)
        //{
        //    return (new Uri(FullUrl(url), UriKind.Absolute)).DnsSafeHost;
        //}

        ///// <summary>
        ///// `hash` would just return the part after the `#` (without the `#`) - if not provided, empty string
        ///// </summary>
        ///// <param name="url"></param>
        ///// <returns></returns>
        //protected string Hash(string url)
        //{
        //    return (new UrlParts(FullUrl(url))).Fragment;
        //}

        ///// <summary>
        ///// `query` would return the part after the `?` (without the `?`- if not provided, empty string
        /////     - if no url was provided and there are magical query params (like in DNN), these would not be returned, but not dnn-internals like tabid or language
        ///// </summary>
        ///// <param name="url"></param>
        ///// <returns></returns>
        //protected string Query(string url)
        //{
        //    return new UrlParts(FullUrl(url)).Query;
        //}

        ///// <summary>
        ///// `suffix` would return the entire suffix starting from the `?` _including_ the `?` or `#` - if nothing is there, empty string
        ///// </summary>
        ///// <param name="url"></param>
        ///// <returns></returns>
        //protected string Suffix(string url)
        //{
        //    return (new UrlParts(FullUrl(url))).Suffix();
        //}
    }
}