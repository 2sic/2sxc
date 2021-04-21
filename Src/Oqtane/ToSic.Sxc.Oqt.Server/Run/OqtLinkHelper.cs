using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic.CompilerServices;
using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Shared;
using ToSic.Custom;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Web;
using Log = ToSic.Eav.Logging.Simple.Log;

namespace ToSic.Sxc.Oqt.Server.Run
{
    /// <summary>
    /// The DNN implementation of the <see cref="ILinkHelper"/>.
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public class OqtLinkHelper : IOqtLinkHelper, IHasLog
    {
        public Razor12 RazorPage { get; set; }
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITenantRepository _tenantRepository;
        private readonly IAliasRepository _aliasRepository;
        private readonly IPageRepository _pageRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly SiteState _siteState;

        public OqtLinkHelper(
            IHttpContextAccessor httpContextAccessor,
            ITenantRepository tenantRepository,
            IAliasRepository aliasRepository,
            IPageRepository pageRepository,
            ISiteRepository siteRepository,
            SiteState siteState,
            IValueConverter OqtValueConverter
            )
        {
            Log = new Log("OqtLinkHelper");

            _httpContextAccessor = httpContextAccessor;
            _tenantRepository = tenantRepository;
            _aliasRepository = aliasRepository;
            _pageRepository = pageRepository;
            _siteRepository = siteRepository;
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

            //var tenantResolver =
            //    new TenantResolver(_httpContextAccessor, _aliasRepository, _tenantRepository, _siteState);
            //var tenant = tenantResolver.GetTenant();
            //var alias2 = tenantResolver.GetAlias();
            var siteId = RazorPage._DynCodeRoot.CmsContext.Site.Id;
            var site = ((OqtSite)RazorPage._DynCodeRoot.CmsContext.Site).UnwrappedContents;
            //var site = _siteRepository.GetSite(siteId);
            var alias = _aliasRepository.GetAliases().FirstOrDefault(a => a.SiteId == siteId);
            var page2sxc = RazorPage._DynCodeRoot.CmsContext.Page;
            var currentPageId = RazorPage._DynCodeRoot.CmsContext.Page.Id;
            //var page = _pageRepository.GetPage(pageId ?? currentPageId);

            var r = Utilities.NavigateUrl(alias.Path, "a11", parameters);
            return r;

            //Tenant tenant = _tenantResolver.GetTenant();
            //var alias = _siteState?.Alias; //_tenantResolver.GetAlias();
            //var page = _pageRepository.GetPage(pageId ?? 0);

            //var r = Utilities.NavigateUrl(alias?.Path ?? "", page?.Path ?? "", parameters);
            //return r;

            //return parametersToUse == null
            //    ? _dnn.Tab.FullUrl
            //    : DotNetNuke.Common.Globals.NavigateURL(targetPage, "", parametersToUse);

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
