using Oqtane.Repository;
using Oqtane.Shared;
using System;
using System.Linq;
using Custom.Hybrid;
using Microsoft.AspNetCore.Http;
using Oqtane.Models;
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Lazy<IAliasRepository> _aliasRepositoryLazy;
        private readonly IPageRepository _pageRepository;
        private readonly SiteState _siteState;

        public OqtLinkHelper(
            IHttpContextAccessor httpContextAccessor,
            Lazy<IAliasRepository> aliasRepositoryLazy,
            IPageRepository pageRepository,
            SiteState siteState
        )
        {
            Log = new Log("OqtLinkHelper");
            // TODO: logging

            _httpContextAccessor = httpContextAccessor;
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

            // It looks that SiteState.Alias have wrong alias, so we can't use it. Try to get right one for link helper.
            var alias = GetRightAlias();

            // This is workaround to get PageRepository working, because it depends on TenantResolver
            // that is not working in this context (it works in api controllers).
            _siteState.Alias = alias;

            var currentPageId = RazorPage._DynCodeRoot?.CmsContext?.Page?.Id;

            var pid = pageId ?? currentPageId;
            if (pid == null)
                throw new Exception($"Error, PageId is unknown, pageId: {pageId}, currentPageId: {currentPageId} .");

            var page = _pageRepository.GetPage(pid.Value);

            return Oqtane.Shared.Utilities.NavigateUrl(alias.Path, page.Path, parameters);
        }

        private Alias GetRightAlias()
        {
            // This httpContext is from _blazor, so we can use host, but can't use path (it is always _blazor).
            var request = _httpContextAccessor?.HttpContext?.Request;
            if (request == null)
                throw new Exception("Error, HttpContext is unknown.");

            var url = $"{request.Host}";

            var siteId = RazorPage._DynCodeRoot?.CmsContext?.Site?.Id;
            if (siteId == null)
                throw new Exception("Error, SiteId is unknown.");

            // Get right Alias.
            var alias = _aliasRepositoryLazy.Value.GetAliases()
                .OrderByDescending(a => a.Name.Length)
                .ThenBy(a => a.Name)
                .FirstOrDefault(a => a.SiteId == siteId && a.Name.StartsWith(url, StringComparison.InvariantCultureIgnoreCase));

            if (alias == null)
                throw new Exception("Error, Alias is unknown.");

            return alias;
        }

        /// <inheritdoc />
        public string Base()
        {
            // helper to generate a base path which is also valid on home (special DNN behaviour)
            const string randomxyz = "this-should-never-exist-in-the-url";
            var basePath = To(parameters: randomxyz + "=1");
            return basePath.Substring(0, basePath.IndexOf(randomxyz, StringComparison.Ordinal));
        }

        public string Api(string noParameterOrder = Eav.Constants.RandomProtectionParameter, string path = null)
        {
            // TODO: STV
            // 1. if path starts with / remove that
            // 2. if it starts with "app/" or "api/" or "some-edition/api" or "app/some-edition/api" should always behave the same
            // 3. should then return a full link (without domain) to the app endpoint
            // Make sure to access an object or code which already does this work, like the stuff which generates the in-page js context or something
            throw new NotImplementedException();
        }
    }
}
