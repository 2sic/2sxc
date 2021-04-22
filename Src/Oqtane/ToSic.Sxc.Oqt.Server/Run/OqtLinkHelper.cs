using Oqtane.Repository;
using Oqtane.Shared;
using System;
using System.Linq;
using ToSic.Custom;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
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
        private readonly Lazy<IAliasRepository> _aliasRepositoryLazy;
        private readonly IPageRepository _pageRepository;
        private readonly SiteState _siteState;

        public OqtLinkHelper(
            Lazy<IAliasRepository> aliasRepositoryLazy,
            IPageRepository pageRepository,
            SiteState siteState
        )
        {
            Log = new Log("OqtLinkHelper");
            // TODO: logging

            _aliasRepositoryLazy = aliasRepositoryLazy;
            _pageRepository = pageRepository;
            _siteState = siteState;
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

            var siteId = RazorPage._DynCodeRoot?.CmsContext?.Site?.Id;
            if (siteId == null)
                throw new Exception("Error, SiteId is unknown.");

            // HACK: This is workaround to get PageRepository working, because it depends on TenantResolver
            // that is not working in this context (it works in api controllers).
            _siteState.Alias ??= _aliasRepositoryLazy.Value.GetAliases().FirstOrDefault(a => a.SiteId == siteId);

            var alias = _siteState.Alias;
            if (alias == null)
                throw new Exception("Error, Alias is unknown.");

            var currentPageId = RazorPage._DynCodeRoot?.CmsContext?.Page?.Id;

            var pid = pageId ?? currentPageId;
            if (pid == null)
                throw new Exception($"Error, PageId is unknown, pageId: {pageId}, currentPageId: {currentPageId} .");

            var page = _pageRepository.GetPage(pid.Value);

            return Utilities.NavigateUrl(alias.Path, page.Path, parameters);
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
