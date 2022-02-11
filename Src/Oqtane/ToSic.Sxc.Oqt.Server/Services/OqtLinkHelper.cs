using Custom.Hybrid;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Oqtane.Repository;
using Oqtane.Shared;
using System;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Code;
using ToSic.Sxc.Images;
using ToSic.Sxc.Oqt.Server.Plumbing;
using ToSic.Sxc.Oqt.Server.Run;
using ToSic.Sxc.Run;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Oqt.Server.Services
{
    /// <summary>
    /// The Oqtane implementation of the <see cref="ILinkHelper"/>.
    /// </summary>
    [PrivateApi]
    public class OqtLinkHelper : LinkHelperBase
    {
        public Razor12 RazorPage { get; set; }
        private readonly IPageRepository _pageRepository;
        private readonly SiteStateInitializer _siteStateInitializer;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly OqtLinkPaths _linkPaths;
        private Sxc.Context.IContextOfBlock _context;

        public OqtLinkHelper(
            IPageRepository pageRepository,
            SiteStateInitializer siteStateInitializer,
            IHttpContextAccessor contextAccessor,
            ILinkPaths linkPaths,
            ImgResizeLinker imgLinker
        ) : base(imgLinker)
        {
            _pageRepository = pageRepository;
            _siteStateInitializer = siteStateInitializer;
            _contextAccessor = contextAccessor;
            _linkPaths = linkPaths as OqtLinkPaths;
        }


        public override void ConnectToRoot(IDynamicCodeRoot codeRoot)
        {
            base.ConnectToRoot(codeRoot);
            _context = codeRoot.Block?.Context;
        }
        
        protected override string ToApi(string api, string parameters = null) => ApiNavigateUrl(api, parameters);
        protected override string ToPage(int? pageId, string parameters = null, string language = null) => PageNavigateUrl(pageId, parameters);
        
        // Prepare Api link.
        private string ApiNavigateUrl(string api, string parameters)
        {
            var alias = _siteStateInitializer.InitializedState.Alias;
            
            var pathWithQueryString = CombineApiWithQueryString(
                _linkPaths.ApiFromSiteRoot(App.Folder, api),
                parameters);

            var relativePath = string.IsNullOrEmpty(alias.Path)
                ? pathWithQueryString
                : $"/{alias.Path}{pathWithQueryString}";

            return relativePath;
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

            // if pageId is invalid, fallback to currentPageId
            if (page == null && currentPageId.HasValue && pid != currentPageId)
                page = _pageRepository.GetPage(currentPageId.Value);

            var alias = _siteStateInitializer.InitializedState.Alias;

            // for invalid page numbers just skip that part 
            var relativePath = Utilities.NavigateUrl(alias.Path, page?.Path ?? string.Empty, parameters ?? string.Empty); // NavigateUrl do not works with absolute links

            return absoluteUrl ? $"{GetCurrentLinkRoot()}{relativePath}" : relativePath;
        }

        public override string GetCurrentLinkRoot()
        {
            var scheme = _contextAccessor?.HttpContext?.Request?.Scheme ?? "http";
            var alias = _siteStateInitializer.InitializedState.Alias;
            var domainName = string.IsNullOrEmpty(alias.Path)
                ? alias.Name
                : alias.Name.Substring(0, alias.Name.Length - alias.Path.Length - 1);
            return  $"{scheme}://{domainName}";
        }

        public override string GetCurrentRequestUrl()
        {
            return _contextAccessor?.HttpContext?.Request?.GetEncodedUrl() ?? string.Empty;
        }
    }
}
