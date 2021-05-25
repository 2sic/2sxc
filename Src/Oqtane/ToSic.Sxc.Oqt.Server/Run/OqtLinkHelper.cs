using Custom.Hybrid;
using Oqtane.Repository;
using Oqtane.Shared;
using System;
using ToSic.Eav.Documentation;
using ToSic.Eav.Helpers;
using ToSic.Eav.Logging;
using ToSic.Sxc.Oqt.Server.Block;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Run;
using ToSic.Sxc.Web;
using ToSic.Sxc.Web.WebApi;
using Log = ToSic.Eav.Logging.Simple.Log;

namespace ToSic.Sxc.Oqt.Server.Run
{
    /// <summary>
    /// The Oqtane implementation of the <see cref="ILinkHelper"/>.
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public class OqtLinkHelper : IOqtLinkHelper, IHasLog
    {
        public Razor12 RazorPage { get; set; }
        private readonly IPageRepository _pageRepository;
        private readonly SiteStateInitializer _siteStateInitializer;
        private readonly OqtLinkPaths _linkPaths;

        public OqtLinkHelper(
            IPageRepository pageRepository,
            SiteStateInitializer siteStateInitializer,
            ILinkPaths linkPaths
        )
        {
            Log = new Log("OqtLinkHelper");
            // TODO: logging

            _pageRepository = pageRepository;
            _siteStateInitializer = siteStateInitializer;
            _linkPaths = linkPaths as OqtLinkPaths;
        }

        public ILinkHelper Init(Razor12 razorPage)
        {
            RazorPage = razorPage;
            return this;
        }

        public ILog Log { get; }

        /// <inheritdoc />
        public string To(string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter, int? pageId = null, string parameters = null, string api = null)
        {
            // prevent incorrect use without named parameters
            Eav.Constants.ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder, $"{nameof(To)}", $"{nameof(pageId)},{nameof(parameters)},{nameof(api)}");

            // Check initial conflicting values.
            if (pageId != null && api != null)
                throw new ArgumentException($"Multiple properties like '{nameof(api)}' or '{nameof(pageId)}' have a value - only one can be provided.");

            // Page or Api?
            return api == null ? PageNavigateUrl(pageId, parameters) : ApiNavigateUrl(api, parameters);
        }

        // Prepare Api link.
        private string ApiNavigateUrl(string api, string parameters)
        {
            var alias = _siteStateInitializer.InitializedState.Alias;
            
            var pathWithQueryString = LinkHelpers.CombineApiWithQueryString(
                _linkPaths.ApiFromSiteRoot(RazorPage.App.Folder, api).TrimPrefixSlash(),
                parameters);

            return $"{alias.Path}/{pathWithQueryString}";
        }

        // Prepare Page link.
        private string PageNavigateUrl(int? pageId, string parameters)
        {
            // Use current pageId, if pageId is not specified.
            var currentPageId = RazorPage._DynCodeRoot?.CmsContext?.Page?.Id;
            var pid = pageId ?? currentPageId;
            if (pid == null)
                throw new Exception($"Error, PageId is unknown, pageId: {pageId}, currentPageId: {currentPageId} .");

            var page = _pageRepository.GetPage(pid.Value);

            var alias = _siteStateInitializer.InitializedState.Alias;

            return Utilities.NavigateUrl(alias.Path, page.Path, parameters ?? string.Empty);
        }

        /// <inheritdoc />
        public string Base()
        {
            // helper to generate a base path which is also valid on home (special DNN behaviour)
            const string randomxyz = "this-should-never-exist-in-the-url";
            var basePath = To(parameters: randomxyz + "=1");
            return basePath.Substring(0, basePath.IndexOf(randomxyz, StringComparison.Ordinal));
        }
    }
}
