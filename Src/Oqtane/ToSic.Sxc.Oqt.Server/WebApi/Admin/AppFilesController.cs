using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Assets;
using ToSic.Eav.WebApi.Routing;
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
    [Route(WebApiConstants.ApiRootWithNoLang + $"/{AreaRoutes.Admin}")]
    [Route(WebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Admin}")]
    [Route(WebApiConstants.ApiRootPathNdLang + $"/{AreaRoutes.Admin}")]

    // Beta routes - TODO: @STV - why is this beta?
    [Route(WebApiConstants.WebApiStateRoot + $"/{AreaRoutes.Admin}")]
    public class AppFilesController : OqtStatefulControllerBase<DummyControllerReal>, IAppFilesController
    {
        public AppFilesController(Lazy<AppAssetsBackend> appAssetsLazy): base("Assets")
        {
            _appAssetsLazy = appAssetsLazy;
        }
        private readonly Lazy<AppAssetsBackend> _appAssetsLazy;

        private AppAssetsBackend Backend() => _appAssetsLazy.Value.Init(Log);


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
        /// <param name="templateKey"></param>
        /// <returns></returns>
        [Obsolete("This Method is Deprecated", false)]
        [HttpPost]
        public bool Create(
            [FromQuery] int appId,
            [FromQuery] string path,
            [FromBody] FileContentsDto content, // note: as of 2020-09 the content is never submitted
            [FromQuery] bool global,
            [FromQuery] string templateKey // as of 2021-12, all create calls include templateKey
            //[FromUri] string purpose = Purpose.Auto
        ) => Backend().Create(new AssetFromTemplateDto
        {
            AppId = appId,
            Path = path,
            Global = global,
            TemplateKey = templateKey,
        });

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
        /// <param name="purpose">filter by Purpose when provided</param>
        /// <returns></returns>
        [HttpGet]
        public TemplatesDto GetTemplates(string purpose = null, string type = null) => Backend().GetTemplates(purpose, type);

        [HttpGet]
        public TemplatePreviewDto Preview(int appId, string path, string templateKey, bool global = false)
            => Backend().GetPreview(appId, path, templateKey, global);

        [HttpGet]
        public AllFilesDto AppFiles(int appId, [FromQuery] string path = null, [FromQuery] string mask = null) => Backend().AppFiles(appId, path, mask);

    }
}