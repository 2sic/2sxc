using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.Configuration;
using ToSic.Eav.WebApi.Admin.Features;

namespace ToSic.Sxc.Dnn.WebApi.Admin
{
    /// <summary>
    /// Provide information about activated features which will be managed externally. 
    /// </summary>
    /// <remarks>
    /// Added in 2sxc 10
    /// </remarks>
    [SupportedModules(DnnSupportedModuleNames)]
    [ValidateAntiForgeryToken]
    public class FeatureController : DnnApiControllerWithFixes<FeatureControllerReal>, IFeatureController
    {
        public FeatureController(): base(FeatureControllerReal.LogSuffix) { }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public FeatureState Details(string nameId) => SysHlp.Real.Details(nameId);

        /// <summary>
        /// POST updated features JSON configuration.
        /// </summary>
        /// <remarks>
        /// Added in 2sxc 13
        /// </remarks>
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
        public bool SaveNew([FromBody] List<FeatureManagementChange> changes) => SysHlp.Real.SaveNew(changes);
    }
}