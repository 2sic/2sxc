using Custom.Hybrid;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Oqtane.Repository;
using Oqtane.Shared;
using System;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Code;
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
    public class OqtLinkHelper : LinkHelper
    {
        public Razor12 RazorPage { get; set; }
        private readonly IPageRepository _pageRepository;
        private readonly SiteStateInitializer _siteStateInitializer;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly OqtLinkPaths _linkPaths;
        private Context.IContextOfBlock _context;

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


        public override void AddBlockContext(IDynamicCodeRoot codeRoot)
        {
            base.AddBlockContext(codeRoot);
            _context = codeRoot.Block?.Context;
        }

        ///// <inheritdoc />
        //public override string To(string noParamOrder = Parameters.Protector, int? pageId = null, object parameters = null, string api = null)
        //{
        //    // prevent incorrect use without named parameters
        //    Parameters.ProtectAgainstMissingParameterNames(noParamOrder, $"{nameof(To)}", $"{nameof(pageId)},{nameof(parameters)},{nameof(api)}");

        //    // Check initial conflicting values.
        //    if (pageId != null && api != null)
        //        throw new ArgumentException($"Multiple properties like '{nameof(api)}' or '{nameof(pageId)}' have a value - only one can be provided.");

        //    var strParams = ParametersToString(parameters);

        //    // Page or Api?
        //    return api == null ? PageNavigateUrl(pageId, strParams) : ApiNavigateUrl(api, strParams);
        //}

        protected override string ToImplementation(int? pageId = null, string parameters = null, string api = null)
        {
            // Page or Api?
            return api == null ? PageNavigateUrl(pageId, parameters) : ApiNavigateUrl(api, parameters);

        }

        // Prepare Api link.
        private string ApiNavigateUrl(string api, string parameters)
        {
            var alias = _siteStateInitializer.InitializedState.Alias;
            
            var pathWithQueryString = LinkHelpers.CombineApiWithQueryString(
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

            return absoluteUrl ? $"{GetDomainName()}{relativePath}" : relativePath;
        }

        public override string GetDomainName()
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
