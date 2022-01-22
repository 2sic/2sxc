using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Infrastructure;
using Oqtane.Shared;
using System.Collections.Generic;
using ToSic.Eav.Configuration;
using ToSic.Eav.Context;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Server.Integration;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Run;
using ToSic.Sxc.WebApi.Features;

namespace ToSic.Sxc.Oqt.Server.WebApi.Admin
{
    [ValidateAntiForgeryToken]

    // Release routes
    [Route(WebApiConstants.ApiRoot + "/admin/[controller]/[action]")]
    [Route(WebApiConstants.ApiRoot2 + "/admin/[controller]/[action]")]
    [Route(WebApiConstants.ApiRoot3 + "/admin/[controller]/[action]")]

    // Beta routes
    [Route(WebApiConstants.WebApiStateRoot + "/admin/[controller]/[action]")]
    public class FeatureController : OqtStatefulControllerBase, IFeatureController
    {
        private readonly FeaturesBackend _featuresBackend;
        private readonly ISite _site;
        private readonly IConfigManager _configManager;
        private readonly OqtModuleHelper _oqtModuleHelper;
        private readonly RemoteRouterLink _remoteRouterLink;
        protected override string HistoryLogName => "Api.Feats";

        public FeatureController(FeaturesBackend featuresBackend, ISite site, IConfigManager configManager, OqtModuleHelper oqtModuleHelper, RemoteRouterLink remoteRouterLink)
        {
            _featuresBackend = featuresBackend;
            _site = site;
            _configManager = configManager;
            _oqtModuleHelper = oqtModuleHelper;
            _remoteRouterLink = remoteRouterLink;
        }

        /// <summary>
        /// Used to be GET System/Features
        /// </summary>
        [HttpGet]
        [Authorize(Roles = RoleNames.Admin)]
        public IEnumerable<FeatureState> List(bool reload = false) => _featuresBackend.Init(Log).GetAll(reload);

        /// <summary>
        /// Used to be GET System/ManageFeaturesUrl
        /// </summary>
        [HttpGet]
        [Authorize(Roles = RoleNames.Host)]
        public string RemoteManageUrl()
        {
            //return "//gettingstarted.2sxc.org/router.aspx?"
            //       + $"DnnVersion={Oqtane.Shared.Constants.Version}"
            //       + $"&2SexyContentVersion={Settings.ModuleVersion}"
            //       + $"&fp={HttpUtility.UrlEncode(Fingerprint.System)}"
            //       + $"&DnnGuid={Guid.Empty}" // we can try to use oqt host user guid from aspnetcore identity
            //       + $"&ModuleId={GetContext().Module.Id}" // needed for callback later on
            //       + "&destination=features";

            var ctx = GetContext();
            var site = ctx.Site;
            var module = ctx.Module;
            
            var link = _remoteRouterLink.LinkToRemoteRouter(RemoteDestinations.Features,
                "Oqt",
                Oqtane.Shared.Constants.Version, // Assembly.GetAssembly(typeof(SiteState))?.GetName().Version?.ToString(4),
                _configManager.GetInstallationId(),
                site,
                module.Id,
                app: null,
                _oqtModuleHelper.IsContentApp(module.Id)
                );
            return link;
        }

        /// <summary>
        /// Used to be GET System/SaveFeatures
        /// </summary>
        [HttpPost]
        [Authorize(Roles = RoleNames.Host)]
        public bool Save([FromBody] FeaturesDto featuresManagementResponse) =>
            _featuresBackend.Init(Log).SaveFeatures(featuresManagementResponse);

    }
}