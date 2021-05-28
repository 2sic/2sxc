using System;
using System.Collections.Generic;
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
using ToSic.Sxc.WebApi.Cms;
using ToSic.Sxc.WebApi.ContentBlocks;
using ToSic.Sxc.WebApi.InPage;

namespace ToSic.Sxc.Dnn.WebApi.Cms
{
    [ValidateAntiForgeryToken]
    // cannot use this, as most requests now come from a lone page [SupportedModules("2sxc,2sxc-app")]
    public class BlockController : SxcApiController, IBlockController<HttpResponseMessage>
    {
        protected override string HistoryLogName => "Api.Block";

        protected CmsRuntime CmsRuntime => _cmsRuntime ?? (_cmsRuntime = base.App == null
                ? null
                : GetService<CmsRuntime>().Init(base.App, true, Log)
            );
        private CmsRuntime _cmsRuntime;

        #region Block

        private ContentBlockBackend Backend => GetService<ContentBlockBackend>().Init(Log);

        private AppViewPickerBackend ViewBackend => GetService<AppViewPickerBackend>().Init(Log);

        /// <summary>
        /// used to be GET Module/GenerateContentBlock
        /// </summary>
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public new string Block(int parentId, string field, int sortOrder, string app = "", Guid? guid = null)
            => Backend.NewBlockAndRender(parentId, field, sortOrder, app, guid);

        #endregion

        #region BlockItems

        /// <inheritdoc />
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void Item([FromUri] int? index = null) => Backend.AddItem(index);

        #endregion


        #region App

        /// <inheritdoc />
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public new void App(int? appId) => ViewBackend.SetAppId(appId);

        /// <inheritdoc />
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<AppUiInfo> Apps(string apps = null) => ViewBackend.Apps(apps);

        #endregion

        #region Types

        /// <inheritdoc />
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<ContentTypeUiInfo> ContentTypes() => ViewBackend.ContentTypes(); //  CmsRuntime?.Views.GetContentTypesWithStatus();

        #endregion

        #region Templates

        /// <inheritdoc />
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<TemplateUiInfo> Templates() => ViewBackend.Templates();

        /// <inheritdoc />
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public Guid? Template(int templateId, bool forceCreateContentGroup) => ViewBackend.SaveTemplateId(templateId, forceCreateContentGroup);

        #endregion


        /// <inheritdoc />
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public HttpResponseMessage Render([FromUri] int templateId, [FromUri] string lang)
        {
            Log.Add($"render template:{templateId}, lang:{lang}");
            try
            {
                var rendered = ViewBackend.Render(templateId, lang);
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

        /// <inheritdoc />
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public bool Publish(string part, int index) => Backend.PublishPart(part, index);
    }
}