using Custom.Hybrid;
using Microsoft.AspNetCore.Http;
using Oqtane.Repository;
using Oqtane.Shared;
using System;
using ToSic.Eav;
using ToSic.Eav.Documentation;
using ToSic.Eav.Helpers;
using ToSic.Eav.Logging;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Run;
using ToSic.Sxc.Web;
using ToSic.Sxc.Web.WebApi;

namespace ToSic.Sxc.Oqt.Server.Run
{
    /// <summary>
    /// The Oqtane implementation of the <see cref="ILinkHelper"/>.
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public class OqtLinkHelper : LinkHelper, ILinkHelper, IHasLog
    {
        public Razor12 RazorPage { get; set; }
        private readonly IPageRepository _pageRepository;
        private readonly SiteStateInitializer _siteStateInitializer;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly OqtLinkPaths _linkPaths;
        //private IApp App;
        private Context.IContextOfBlock _context;

        public OqtLinkHelper(
            IPageRepository pageRepository,
            SiteStateInitializer siteStateInitializer,
            IHttpContextAccessor contextAccessor,
            ILinkPaths linkPaths
        )
        {
            //Log = new Log("OqtLinkHelper");
            // TODO: logging

            _pageRepository = pageRepository;
            _siteStateInitializer = siteStateInitializer;
            _contextAccessor = contextAccessor;
            _linkPaths = linkPaths as OqtLinkPaths;
        }

        public override void Init(Context.IContextOfBlock context, IApp app)
        {
            base.Init(context, app);
            _context = context;
        }
        
        //public ILog Log { get; }

        /// <inheritdoc />
        public override string To(string dontRelyOnParameterOrder = Parameters.Protector, int? pageId = null, string parameters = null, string api = null)
        {
            // prevent incorrect use without named parameters
            Parameters.ProtectAgainstMissingParameterNames(dontRelyOnParameterOrder, $"{nameof(To)}", $"{nameof(pageId)},{nameof(parameters)},{nameof(api)}");

            // Check initial conflicting values.
            if (pageId != null && api != null)
                throw new ArgumentException($"Multiple properties like '{nameof(api)}' or '{nameof(pageId)}' have a value - only one can be provided.");

            // Page or Api?
            return api == null ? PageNavigateUrl(pageId, parameters) : ApiNavigateUrl(api, parameters);
        }

        // Prepare Api link.
        private string ApiNavigateUrl(string api, string parameters, bool absoluteUrl = true)
        {
            var alias = _siteStateInitializer.InitializedState.Alias;
            
            var pathWithQueryString = LinkHelpers.CombineApiWithQueryString(
                _linkPaths.ApiFromSiteRoot(App.Folder, api),
                parameters);

            var relativePath = string.IsNullOrEmpty(alias.Path)
                ? pathWithQueryString
                : $"/{alias.Path}{pathWithQueryString}";

            return absoluteUrl ? $"{GetDomainName()}{relativePath}" : relativePath;
        }

        // Prepare Page link.
        private string PageNavigateUrl(int? pageId, string parameters, bool absoluteUrl = true)
        {
            // Use current pageId, if pageId is not specified.
            var currentPageId = _context?.Page?.Id;
            var pid = pageId ?? currentPageId;
            if (pid == null)
                throw new Exception($"Error, PageId is unknown, pageId: {pageId}, currentPageId: {currentPageId} .");

            var page = _pageRepository.GetPage(pid.Value);

            var alias = _siteStateInitializer.InitializedState.Alias;

            var relativePath = Utilities.NavigateUrl(alias.Path, page.Path, parameters ?? string.Empty); // NavigateUrl do not works with absolute links

            return absoluteUrl ? $"{GetDomainName()}{relativePath}" : relativePath;
        }

        private string GetDomainName()
        {
            var scheme = _contextAccessor?.HttpContext?.Request?.Scheme ?? "http";
            var alias = _siteStateInitializer.InitializedState.Alias;
            var domainName = string.IsNullOrEmpty(alias.Path)
                ? alias.Name
                : alias.Name.Substring(0, alias.Name.Length - alias.Path.Length - 1);
            return  $"{scheme}://{domainName}";
        }
    }
}
