using System;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System.Collections.Generic;
using System.Web.Http;
using ToSic.Sxc.Apps.Assets;
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
    public class AppFilesController : SxcApiControllerBase, IAppFilesController
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
        /// <param name="templateKey"></param>
        /// <returns></returns>
        [HttpPost]
        public bool Create(
            [FromUri] int appId,
            [FromUri] string path,
            [FromBody] FileContentsDto content, // note: as of 2020-09 the content is never submitted
            [FromUri] bool global,
            [FromUri] string templateKey // as of 2021-12, all create calls include templateKey
            //[FromUri] string purpose = Purpose.Auto
            )
            => Backend().Create(new AssetFromTemplateDto
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
        public bool Asset([FromUri] int appId, [FromBody] AssetEditInfo template,
            [FromUri] int templateId = 0, [FromUri] string path = null, // identifier is either template Id or path
            // todo w/SPM - global never seems to be used - must check why and if we remove or add to UI
            // TODO: NEW PARAM TEMPLATEKey SHOULD BE USED TO CREATE THE FILE
            [FromUri] bool global = false) 
            => Backend().Save(appId: appId, template: template, templateId: templateId, global: global, path: path);


        /// <summary>
        /// Get all asset template types
        /// </summary>
        /// <param name="purpose">filter by Purpose when provided</param>
        /// <returns></returns>
        [HttpGet]
        public TemplatesDto GetTemplates(string purpose = null, string type = null) => Backend().GetTemplates(purpose, type);

        [HttpGet]
        public TemplatePreviewDto Preview(int appId, string path, string name, string templateKey, bool global = false)
            => Backend().GetPreview(appId, path, name, templateKey, global);

        /// <summary>
        /// Create a new file from template
        /// </summary>
        /// <param name="assetFromTemplateDto">AssetFromTemplateDto</param>
        /// <returns></returns>
        [HttpPost]
        public bool CreateTemplate(AssetFromTemplateDto assetFromTemplateDto) => Backend().Create(assetFromTemplateDto);

    }
}