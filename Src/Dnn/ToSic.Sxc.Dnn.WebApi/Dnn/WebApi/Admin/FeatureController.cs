using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.WebApi.Admin.Features;
using ToSic.Eav.WebApi.PublicApi;

namespace ToSic.Sxc.Dnn.WebApi.Admin
{
    /// <summary>
    /// Provide information about activated features which will be managed externally. 
    /// </summary>
    /// <remarks>
    /// Added in 2sxc 10
    /// </remarks>
    [SupportedModules("2sxc,2sxc-app")]
    [ValidateAntiForgeryToken]
    public class FeatureController : DnnApiControllerWithFixes<FeatureControllerReal>, IFeatureController
    {
        public FeatureController(): base(FeatureControllerReal.LogSuffix) { }

        // TODO: PROBABLY REMOVE, PROBABLY NOT USED ANY MORE
        ///// <summary>
        ///// Used to be GET a list of Features
        ///// </summary>
        ///// <remarks>
        ///// Added in 2sxc 10
        ///// </remarks>
        //[HttpGet]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
        //public IEnumerable<FeatureState> List(bool reload = false) => Real.List(reload);

        // v13.02 not used any more
        ///// <summary>
        ///// Used to be GET the URL which will open the UI to enable/disable features.
        ///// Note that the implementation for Oqtane will have to be different. 
        ///// </summary>
        ///// <remarks>
        ///// Added in 2sxc 10
        ///// </remarks>
        //[HttpGet]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
        //public string RemoteManageUrl()
        //{
        //    var site = GetService<ISite>();
        //    var module = Request.FindModuleInfo();
        //    var link = GetService<RemoteRouterLink>().LinkToRemoteRouter(RemoteDestinations.Features,
        //        site,
        //        module.ModuleID,
        //        app: null,
        //        module.DesktopModule.ModuleName == "2sxc");
        //    return link;
        //}


        // TODO: PROBABLY REMOVE, PROBABLY NOT USED ANY MORE
        ///// <summary>
        ///// Used to be POST updated features JSON configuration.
        ///// These must always be signed by the 2sxc.org private key.
        ///// </summary>
        ///// <remarks>
        ///// Added in 2sxc 10
        ///// </remarks>
        //[HttpPost]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
        //public bool Save([FromBody] FeaturesDto featuresManagementResponse) => Real.Save(featuresManagementResponse);

        /// <summary>
        /// POST updated features JSON configuration.
        /// </summary>
        /// <remarks>
        /// Added in 2sxc 13
        /// </remarks>
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
        public bool SaveNew([FromBody] List<FeatureNewDto> featuresManagementResponse) => Real.SaveNew(featuresManagementResponse);
    }
}