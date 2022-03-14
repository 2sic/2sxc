using Custom.Hybrid;
using Oqtane.Repository;
using Oqtane.Shared;
using System;
using System.Linq;
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
        private readonly Lazy<IAliasRepository> _aliasRepositoryLazy;
        private Sxc.Context.IContextOfBlock _context;

        public OqtLinkHelper(
            IPageRepository pageRepository,
            SiteStateInitializer siteStateInitializer,
            ImgResizeLinker imgLinker,
            Lazy<ILinkPaths> linkPathsLazy,
            Lazy<IAliasRepository> aliasRepositoryLazy
        ) : base(imgLinker, linkPathsLazy)
        {
            _pageRepository = pageRepository;
            _siteStateInitializer = siteStateInitializer;
            _aliasRepositoryLazy = aliasRepositoryLazy;
        }

        private new OqtLinkPaths LinkPaths => (OqtLinkPaths)base.LinkPaths;

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
                LinkPaths.ApiFromSiteRoot(App.Folder, api),
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

            var page = _pageRepository.GetPage(pid.Value, false);

            if (page == null) return null; 
   
            var alias = _aliasRepositoryLazy.Value.GetAliases()
                    .OrderByDescending(a => /*a.IsDefault*/  a.Name.Length) // TODO: a.IsDefault DESC after upgrade to Oqt v3.0.3+
                    //.ThenByDescending(a => a.Name.Length)
                    .ThenBy(a => a.Name)
                    .FirstOrDefault(a => a.SiteId == page.SiteId);

            if (alias == null)
                throw new Exception($"Error, Alias is unknown, pageId: {pageId}, siteId: {page.SiteId} .");

            // for invalid page numbers just skip that part 
            var relativePath = Utilities.NavigateUrl(alias.Path, page?.Path ?? string.Empty, parameters ?? string.Empty); // NavigateUrl do not works with absolute links

            return absoluteUrl ? $"{LinkPaths.GetCurrentLinkRoot()}{relativePath}" : relativePath;
        }
    }
}
