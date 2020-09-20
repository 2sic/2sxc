using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav;
using ToSic.Sxc.Apps.Assets;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.WebApi.Logging;
using ToSic.Sxc.WebApi.Assets;

namespace ToSic.Sxc.WebApi.Cms
{
    /// <summary>
    /// This one supplies portal-wide (or cross-portal) settings / configuration
    /// </summary>
	[SupportedModules("2sxc,2sxc-app")]
    [DnnLogExceptions]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [ValidateAntiForgeryToken]
    public partial class AppAssetsController : SxcApiControllerBase
    {
        protected override string HistoryLogName => "Api.Assets";

        private AppAssetsBackend Backend() => Factory.Resolve<AppAssetsBackend>().Init(GetBlock().App, new DnnUser(UserInfo), Log);


        [HttpGet]
        public List<string> List(int appId, bool global = false, string path = null, string mask = "*.*", bool withSubfolders = false, bool returnFolders = false) 
            => Backend().List(appId, global, path, mask, withSubfolders, returnFolders);

        /// <summary>
        /// Create a new file (if it doesn't exist yet) and optionally prefill it with content
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="path"></param>
        /// <param name="content"></param>
        /// <param name="global">this determines, if the app-file store is the global in _default or the local in the current app</param>
        /// <returns></returns>
        [HttpPost]
        public bool Create([FromUri] int appId, [FromUri] string path,[FromBody] FileContentsDto content, bool global = false) 
            => Backend().Create(appId, path, content, global);


        /// <summary>
        /// Get details and source code
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="global">this determines, if the app-file store is the global in _default or the local in the current app</param>
        /// <param name="path"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        [HttpGet]
        public AssetEditInfo Asset(int templateId = 0, string path = null, bool global = false, int appId = 0)
            => Backend().Get(templateId, path, global, appId);


        /// <summary>
        /// Update an asset with POST
        /// </summary>
        /// <param name="template"></param>
        /// <param name="templateId"></param>
        /// <param name="global">this determines, if the app-file store is the global in _default or the local in the current app</param>
        /// <param name="path"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        [HttpPost]
        public bool Asset([FromBody] AssetEditInfo template,[FromUri] int templateId = 0, [FromUri] bool global = false, [FromUri] string path = null, [FromUri] int appId = 0) 
            => Backend().Save(template, templateId, global, path, appId);

    }
}