using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using System.Collections.Generic;
using ToSic.Eav.WebApi.Features;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Eav.WebApi.Routing;
using ToSic.Sxc.Oqt.Server.Controllers;

namespace ToSic.Sxc.Oqt.Server.WebApi.Admin
{
    [ValidateAntiForgeryToken]

    // Release routes
    [Route(WebApiConstants.ApiRootWithNoLang + $"/{AreaRoutes.Admin}")]
    [Route(WebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Admin}")]
    [Route(WebApiConstants.ApiRootPathNdLang + $"/{AreaRoutes.Admin}")]

    // Beta routes - TODO: @STV - why is this beta?
    [Route(WebApiConstants.WebApiStateRoot + $"/{AreaRoutes.Admin}")]
    public class FeatureController : OqtStatefulControllerBase<FeatureControllerReal>, IFeatureController
    {
        public FeatureController(/*FeaturesControllerReal featuresBackend, OqtModuleHelper oqtModuleHelper, RemoteRouterLink remoteRouterLink*/): base(FeatureControllerReal.LogSuffix)
        {
            //_featuresBackend = featuresBackend;
            //_oqtModuleHelper = oqtModuleHelper;
            //_remoteRouterLink = remoteRouterLink;
        }
        //private readonly FeaturesControllerReal _featuresBackend;
        //private readonly OqtModuleHelper _oqtModuleHelper;
        //private readonly RemoteRouterLink _remoteRouterLink;


        // TODO: PROBABLY REMOVE, PROBABLY NOT USED ANY MORE
        ///// <summary>
        ///// Used to be GET System/Features
        ///// </summary>
        //[HttpGet]
        //[Authorize(Roles = RoleNames.Admin)]
        //public IEnumerable<FeatureState> List(bool reload = false) => Real.List(reload);

        // v13.02 not used any more
        ///// <summary>
        ///// Used to be GET System/ManageFeaturesUrl
        ///// </summary>
        //[HttpGet]
        //[Authorize(Roles = RoleNames.Host)]
        //public string RemoteManageUrl()
        //{
        //    var ctx = GetContext();
        //    var site = ctx.Site;
        //    var module = ctx.Module;

        //    var link = _remoteRouterLink.LinkToRemoteRouter(RemoteDestinations.Features,
        //        site,
        //        module.Id,
        //        app: null,
        //        _oqtModuleHelper.IsContentApp(module.Id)
        //        );
        //    return link;
        //}

        // TODO: PROBABLY REMOVE, PROBABLY NOT USED ANY MORE
        ///// <summary>
        ///// Used to be GET System/SaveFeatures
        ///// </summary>
        //[HttpPost]
        //[Authorize(Roles = RoleNames.Host)]
        //public bool Save([FromBody] FeaturesDto featuresManagementResponse) => Real.Save(featuresManagementResponse);

        /// <summary>
        /// POST updated features JSON configuration.
        /// </summary>
        /// <remarks>
        /// Added in 2sxc 13
        /// </remarks>
        [HttpPost]
        [Authorize(Roles = RoleNames.Host)]
        public bool SaveNew([FromBody] List<FeatureNewDto> featuresManagementResponse) => Real.SaveNew(featuresManagementResponse);

    }
}