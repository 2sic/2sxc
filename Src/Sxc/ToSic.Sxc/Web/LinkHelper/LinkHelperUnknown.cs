using System;
using ToSic.Eav.Documentation;
using ToSic.Eav.Run;
using ToSic.Eav.Run.Unknown;
using ToSic.Sxc.Images;
using ToSic.Sxc.Run;

namespace ToSic.Sxc.Web
{
    [PrivateApi("for testing / un-implemented use")]
    public class LinkHelperUnknown: LinkHelperBase, IIsUnknown
    {
        public const string DefDomain = "unknown.2sxc.org";
        public const string DefProtocol = "https";
        public const string DefRoot = DefProtocol +"://" + DefDomain;
        public const string NiceCurrentPath = "/folder/sub-folder/";
        public const string NiceCurrentPage = "current-page";
        public const string NiceCurrentRelative = NiceCurrentPath + NiceCurrentPage;

        // 2dm - disable this, as there cannot be a current query on a page, unless it's page-url like tabid=25
        public const string CurrentQuery = "";// "testparam=2742";
        public const string NiceCurrentUrlRoot = DefRoot + NiceCurrentRelative;
        public const string NiceCurrentUrl = NiceCurrentUrlRoot;// + "?" + CurrentQuery;

        public const string UglyAnyQuery = "tabId={0}";
        public const string UglyCurrentQuery = "tabId=27";
        public const string UglyCurrentPage = "default.aspx?" + UglyCurrentQuery;
        //public const string UglyCurrentRelative = NiceCurrentPath + NiceCurrentPage;
        public const string UglyAnyPage = "default.aspx?" + UglyAnyQuery;
        public const string UglyCurrentUrl = DefRoot + "/" + UglyCurrentPage;
        public const string UglyAnyPageUrl = DefRoot + "/" + UglyAnyPage;

        public const string NiceAnyRelative = "/page{0}";
        public static string NiceAnyPageUrl = DefRoot + NiceAnyRelative;

        internal static string CurrentPageUrl = NiceCurrentUrl;
        internal static string AnyPageUrl = NiceAnyPageUrl;

        public LinkHelperUnknown(ImgResizeLinker imgLinker, Lazy<ILinkPaths> linkPathsLazy, WarnUseOfUnknown<LinkHelperUnknown> warn) : base(imgLinker, linkPathsLazy)
        {
        }

        protected override string ToApi(string api, string parameters = null) => $"{api}{Parameters(parameters)}";

        protected override string ToPage(int? pageId, string parameters = null, string language = null) =>
            // Page or Api?
            pageId != null
                ? string.Format(AnyPageUrl, pageId) + Parameters(parameters)
                : $"{CurrentPageUrl}{Parameters(parameters)}";

        private static string Parameters(string parameters) => string.IsNullOrEmpty(parameters) ? parameters : $"?{parameters}";


        public static void SwitchModeToUgly(bool uglyOn)
        {
            CurrentPageUrl = uglyOn ? UglyCurrentUrl : NiceCurrentUrl;
            AnyPageUrl = uglyOn ? UglyAnyPageUrl : NiceAnyPageUrl;
        }

        //protected override string ToImplementation(int? pageId = null, string parameters = null, string api = null)
        //{
        //    if (!string.IsNullOrEmpty(parameters)) parameters = $"?{parameters}";

        //    // Page or Api?
        //    return api == null 
        //        ? pageId != null
        //            ? $"{GetDomainName()}/page{pageId}{parameters}" 
        //            : $"{CurrentPage}{parameters}"
        //        : $"{api}{parameters}";
        //}
    }
}
