using Custom.Hybrid;
using Oqtane.Repository;
using Oqtane.Shared;
using System;
using ToSic.Eav.Documentation;
using ToSic.Eav.Helpers;
using ToSic.Eav.Logging;
using ToSic.Sxc.Oqt.Server.Block;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Web;
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

        public OqtLinkHelper(
            IPageRepository pageRepository,
            SiteStateInitializer siteStateInitializer
        )
        {
            Log = new Log("OqtLinkHelper");
            // TODO: logging

            _pageRepository = pageRepository;
            _siteStateInitializer = siteStateInitializer;
        }

        public ILinkHelper Init(Razor12 razorPage)
        {
            RazorPage = razorPage;
            return this;
        }

        public ILog Log { get; }

        /// <inheritdoc />
        public string To(string requiresNamedParameters = null, int? pageId = null, string parameters = null)
        {
            // prevent incorrect use without named parameters
            if (requiresNamedParameters != null)
                throw new Exception("The Link.To can only be used with named parameters. try Link.To( parameters: \"tag=daniel&sort=up\") instead.");

            var alias = _siteStateInitializer.InitializedState.Alias;

            var currentPageId = RazorPage._DynCodeRoot?.CmsContext?.Page?.Id;

            var pid = pageId ?? currentPageId;
            if (pid == null)
                throw new Exception($"Error, PageId is unknown, pageId: {pageId}, currentPageId: {currentPageId} .");

            var page = _pageRepository.GetPage(pid.Value);

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

        public string Api(string dontRelyOnParameterOrder = Eav.Constants.RandomProtectionParameter, string path = null)
        {
            Eav.Constants.ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder, "Api", $"{nameof(path)}");

            if (string.IsNullOrEmpty(path)) return string.Empty;

            path = path.ForwardSlash();
            path = path.TrimPrefixSlash();

            //if (path.PrefixSlash().ToLowerInvariant().Contains("/app/"))
            //    throw new ArgumentException("Error, path shouldn't have \"app\" part in it. It is expected to be relative to application root.");

            //if (!path.PrefixSlash().ToLowerInvariant().Contains("/api/"))
            //    throw new ArgumentException("Error, path should have \"api\" part in it.");

            // TODO: build url with 'app'/'applicationName'
            
            // TODO: centralize how the API path is calculated
            var siteRoot = OqtAssetsAndHeaders.GetSiteRoot(_siteStateInitializer.InitializedState).TrimLastSlash();
            return $"{siteRoot}/app/{RazorPage.App.Folder}/{path}";
        }
    }
}
