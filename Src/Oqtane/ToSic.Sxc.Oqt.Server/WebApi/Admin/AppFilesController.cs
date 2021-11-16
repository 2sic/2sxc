using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Sxc.Apps.Assets;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.WebApi.Assets;

namespace ToSic.Sxc.Oqt.Server.WebApi.Admin
{
    /// <summary>
    /// This one supplies portal-wide (or cross-portal) settings / configuration
    /// </summary>
    [ValidateAntiForgeryToken]
    [Authorize(Roles = RoleNames.Admin)]

    // Release routes
    [Route(WebApiConstants.ApiRoot + "/admin/[controller]/[action]")]
    [Route(WebApiConstants.ApiRoot2 + "/admin/[controller]/[action]")]
    [Route(WebApiConstants.ApiRoot3 + "/admin/[controller]/[action]")]

    // Beta routes
    [Route(WebApiConstants.WebApiStateRoot + "/admin/[controller]/[action]")]
    public class AppFilesController : OqtStatefulControllerBase
    {
        private readonly Lazy<AppAssetsBackend> _appAssetsLazy;
        protected override string HistoryLogName => "Api.Assets";

        private AppAssetsBackend Backend() => _appAssetsLazy.Value.Init(Log);

        public AppFilesController(Lazy<AppAssetsBackend> appAssetsLazy)
        {
            _appAssetsLazy = appAssetsLazy;
        }

        [HttpGet]
        public List<string> All(
            [FromQuery] int appId,
            [FromQuery] bool global,
            [FromQuery] string path = null,
            [FromQuery] string mask = "*.*",
            [FromQuery] bool withSubfolders = false,
            [FromQuery] bool returnFolders = false
        ) => Backend().List(appId, global, path, mask, withSubfolders, returnFolders);

        /// <summary>
        /// Get details and source code
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="global">this determines, if the app-file store is the global in _default or the local in the current app</param>
        /// <param name="path"></param>
        /// <param name="appId"></param>
        /// <returns></returns>
        [HttpGet]
        public AssetEditInfo Asset(
            [FromQuery] int appId,
            [FromQuery] int templateId = 0,
            [FromQuery] string path = null, // identifier is always one of these two
            [FromQuery] bool global = false
        ) => Backend().Get(appId, templateId, path, global);

        /// <summary>
        /// Create a new file (if it doesn't exist yet) and optionally prefill it with content
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="path"></param>
        /// <param name="content"></param>
        /// <param name="global">this determines, if the app-file store is the global in _default or the local in the current app</param>
        /// <param name="purpose">auto;razor;token;api;search</param>
        /// <returns></returns>
        [Obsolete("This Method is Deprecated", false)]
        [HttpPost]
        public bool Create(
            [FromQuery] int appId,
            [FromQuery] string path,
            [FromBody] FileContentsDto content, // note: as of 2020-09 the content is never submitted
            [FromQuery] bool global,
            [FromQuery] string purpose = Purpose.Auto
        ) => Backend().Create(appId, path, content, purpose, global);

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
        public bool Asset(
            [FromQuery] int appId,
            [FromBody] AssetEditInfo template,
            [FromQuery] int templateId = 0,
            [FromQuery] string path = null, // identifier is either template Id or path
            // todo w/SPM - global never seems to be used - must check why and if we remove or add to UI
            [FromQuery] bool global = false
        ) => Backend().Save(appId, template, templateId, global, path);


        /// <summary>
        /// Get all asset template types
        /// </summary>
        /// <param name="templateKey"></param>
        /// <returns></returns>
        [HttpGet]
        public TemplatesDto GetTemplates(string templateKey = null) => Backend().GetTemplates(templateKey);

        /// <summary>
        /// Create a new file from template
        /// </summary>
        /// <param name="assetFromTemplateDto">AssetFromTemplateDto</param>
        /// <returns></returns>
        [HttpPost]
        public bool CreateTemplate(AssetFromTemplateDto assetFromTemplateDto) => Backend().Create(assetFromTemplateDto);
    }
}