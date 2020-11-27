using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using ToSic.Eav.Configuration;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.Oqt.Shared.Dev;
using ToSic.Sxc.WebApi.Features;

namespace ToSic.Sxc.Oqt.Server.Controllers.Admin
{
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    [Route(WebApiConstants.WebApiStateRoot + "/admin/[controller]/[action]")]
    public class FeatureController : SxcStatefulControllerBase, IFeatureController
    {
        private readonly FeaturesBackend _featuresBackend;
        protected override string HistoryLogName => "Api.Feats";

        public FeatureController(StatefulControllerDependencies dependencies, FeaturesBackend featuresBackend) : base(dependencies)
        {
            _featuresBackend = featuresBackend;
        }

        /// <summary>
        /// Used to be GET System/Features
        /// </summary>
        [HttpGet]
        [Authorize(Roles = Oqtane.Shared.Constants.AdminRole)]
        public IEnumerable<Feature> List(bool reload = false) => _featuresBackend.Init(Log).GetAll(reload);

        /// <summary>
        /// Used to be GET System/ManageFeaturesUrl
        /// </summary>
        [HttpGet]
        [Authorize(Roles = Oqtane.Shared.Constants.HostRole)]
        public string RemoteManageUrl()
        {
            WipConstants.DontDoAnythingImplementLater();
            
            return "//gettingstarted.2sxc.org/router.aspx?"
                   + $"DnnVersion={Oqtane.Shared.Constants.Version}"
                   + $"&2SexyContentVersion={Settings.ModuleVersion}"
                   + $"&fp={HttpUtility.UrlEncode(Fingerprint.System)}"
                   + $"&DnnGuid={Guid.Empty}" // we can try to use oqt host user guid from aspnetcore identity
                   + $"&ModuleId={GetContext().Module.Id}" // needed for callback later on
                   + "&destination=features";
        }

        /// <summary>
        /// Used to be GET System/SaveFeatures
        /// </summary>
        [HttpPost]
        [Authorize(Roles = Oqtane.Shared.Constants.HostRole)]
        public bool Save([FromBody] FeaturesDto featuresManagementResponse) =>
            _featuresBackend.Init(Log).SaveFeatures(featuresManagementResponse);
        
    }
}