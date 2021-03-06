﻿using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Sxc.Apps.Assets;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Dnn.WebApi.Logging;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.WebApi.Assets;

namespace ToSic.Sxc.Dnn.WebApi.Admin
{
    /// <summary>
    /// This one supplies portal-wide (or cross-portal) settings / configuration
    /// </summary>
	[SupportedModules("2sxc,2sxc-app")]
    [DnnLogExceptions]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [ValidateAntiForgeryToken]
    public class AppFilesController : SxcApiControllerBase
    {
        protected override string HistoryLogName => "Api.Assets";

        private AppAssetsBackend Backend() => GetService<AppAssetsBackend>().Init(Log);


        [HttpGet]
        public List<string> All(int appId, bool global, string path = null, string mask = "*.*", bool withSubfolders = false, bool returnFolders = false) 
            => Backend().List(appId, global, path, mask, withSubfolders, returnFolders);

        /// <summary>
        /// Get details and source code
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="global">this determines, if the app-file store is the global in _default or the local in the current app</param>
        /// <param name="path"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        [HttpGet]
        public AssetEditInfo Asset(int appId, 
            int templateId = 0, string path = null, // identifier is always one of these two
            bool global = false)
            => Backend().Get(appId, templateId, path, global);


        /// <summary>
        /// Create a new file (if it doesn't exist yet) and optionally prefill it with content
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="path"></param>
        /// <param name="content"></param>
        /// <param name="global">this determines, if the app-file store is the global in _default or the local in the current app</param>
        /// <param name="purpose">auto;razor;token;api;search</param>
        /// <returns></returns>
        [HttpPost]
        public bool Create([FromUri] int appId, [FromUri] string path,
            [FromBody] FileContentsDto content, // note: as of 2020-09 the content is never submitted
            bool global, [FromUri] string purpose = AssetEditor.PurposeType.Auto) 
            => Backend().Create(appId, path, content, purpose, global);




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
        public bool Asset([FromUri] int appId, [FromBody] AssetEditInfo template,
            [FromUri] int templateId = 0, [FromUri] string path = null, // identifier is either template Id or path
            // todo w/SPM - global never seems to be used - must check why and if we remove or add to UI
            [FromUri] bool global = false) 
            => Backend().Save(appId: appId, template: template, templateId: templateId, global: global, path: path);

    }
}