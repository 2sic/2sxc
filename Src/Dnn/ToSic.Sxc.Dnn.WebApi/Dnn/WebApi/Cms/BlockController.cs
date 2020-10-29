using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Api;
using ToSic.Eav.Apps.Ui;
using ToSic.SexyContent.WebApi;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.WebApi.ContentBlocks;
using ToSic.Sxc.WebApi.InPage;

namespace ToSic.Sxc.Dnn.WebApi.Cms
{
    [ValidateAntiForgeryToken]
    // cannot use this, as most requests now come from a lone page [SupportedModules("2sxc,2sxc-app")]
    public class BlockController : SxcApiController
    {
        protected override string HistoryLogName => "Api.Block";

        protected CmsRuntime CmsRuntime => _cmsRuntime ?? (_cmsRuntime = base.App == null ? null : new CmsRuntime(base.App, Log, true, false));
        private CmsRuntime _cmsRuntime;

        #region Block

        private ContentBlockBackend Backend => Eav.Factory.Resolve<ContentBlockBackend>().Init(GetContext(), GetBlock(), Log);


        /// <summary>
        /// used to be GET Module/GenerateContentBlock
        /// </summary>
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public new string Block(int parentId, string field, int sortOrder, string app = "", Guid? guid = null)
        {
            var entityId = Backend.NewBlock(parentId, field, sortOrder, app, guid);

            // now return a rendered instance
            var newContentBlock = new BlockFromEntity().Init(GetBlock(), entityId, Log);
            return newContentBlock.BlockBuilder.Render();

        }
        #endregion

        #region BlockItems

        /// <summary>
        /// used to be GET Module/AddItem
        /// </summary>
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void Item([FromUri] int? index = null) => Backend.AddItem(index);

        #endregion


        #region App

        /// <summary>
        /// used to be GET Module/SetAppId
        /// </summary>
        /// <param name="appId"></param>
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public new void App(int? appId)
            => new AppViewPickerBackend().Init(GetContext(), GetBlock(), Log)
                .SetAppId(appId);

        /// <summary>
        /// used to be GET Module/GetSelectableApps
        /// </summary>
        /// <param name="apps"></param>
        /// <returns></returns>
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<AppUiInfo> Apps(string apps = null)
        {
            // Note: we must get the zone-id from the tenant, since the app may not yet exist when inserted the first time
            var tenant = new DnnSite(PortalSettings);
            return new CmsZones(tenant.ZoneId, Log).AppsRt.GetSelectableApps(tenant, apps).ToList();
        }

        #endregion

        #region Types

        /// <summary>
        /// used to be GET Module/GetSelectableContentTypes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<ContentTypeUiInfo> ContentTypes() => CmsRuntime?.Views.GetContentTypesWithStatus();

        #endregion

        #region Templates

        /// <summary>
        /// used to be GET Module/GetSelectableTemplates
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<TemplateUiInfo> Templates() => CmsRuntime?.Views.GetCompatibleViews(base.App, GetBlock().Configuration);

        /// <summary>
        /// Used in InPage.js
        /// used to be GET Module/SaveTemplateId
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="forceCreateContentGroup"></param>
        /// <returns></returns>
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public Guid? Template(int templateId, bool forceCreateContentGroup)
            => new AppViewPickerBackend().Init(GetContext(), GetBlock(), Log)
                .SaveTemplateId(templateId, forceCreateContentGroup);

        #endregion


        /// <summary>
        /// used to be GET Module/RenderTemplate
        /// js changed
        /// </summary>
        /// <summary>
        /// Used in InPage.js
        /// </summary>
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public HttpResponseMessage Render([FromUri] int templateId, [FromUri] string lang)
        {
            Log.Add($"render template:{templateId}, lang:{lang}");
            try
            {
                var rendered = new AppViewPickerBackend().Init(GetContext(), GetBlock(), Log).Render(templateId, lang);
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(rendered, Encoding.UTF8, "text/plain")
                };
            }
            catch (Exception e)
            {
				Exceptions.LogException(e);
                throw;
            }
        }

        /// <summary>
        /// Used to be GET Module/Publish
        /// </summary>
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public bool Publish(string part, int index) => Backend.PublishPart(part, index);
    }
}