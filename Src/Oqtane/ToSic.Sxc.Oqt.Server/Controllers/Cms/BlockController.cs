using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using ToSic.Eav.Apps.Ui;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.WebApi.ContentBlocks;
using ToSic.Sxc.WebApi.InPage;

namespace ToSic.Sxc.Oqt.Server.Controllers
{
    [Route(WebApiConstants.WebApiStateRoot + "/cms/block/[action]")]
    [ValidateAntiForgeryToken]
    [ApiController]
    // cannot use this, as most requests now come from a lone page [SupportedModules("2sxc,2sxc-app")]
    public class BlockController : SxcStatefulControllerBase
    {
        private readonly Lazy<CmsRuntime> _lazyCmsRuntime;
        private readonly Lazy<ContentBlockBackend> _blockBackendLazy;
        private readonly Lazy<AppViewPickerBackend> _viewPickerBackendLazy;
        private readonly Lazy<CmsZones> _cmsZonesLazy;
        protected override string HistoryLogName => "Api.Block";
        public BlockController(StatefulControllerDependencies dependencies,
            Lazy<CmsRuntime> lazyCmsRuntime, 
            Lazy<ContentBlockBackend> blockBackendLazy,
            Lazy<AppViewPickerBackend> viewPickerBackendLazy,
            Lazy<CmsZones> cmsZonesLazy) : base(dependencies)
        {
            _lazyCmsRuntime = lazyCmsRuntime;
            _blockBackendLazy = blockBackendLazy;
            _viewPickerBackendLazy = viewPickerBackendLazy;
            _cmsZonesLazy = cmsZonesLazy;
        }

        protected CmsRuntime CmsRuntime
        {
            get
            {
                var runtime = _cmsRuntime;
                if (runtime != null) return runtime;

                return _cmsRuntime = ContextApp == null ? null : _lazyCmsRuntime.Value.Init(ContextApp, true, false, Log);
            }
        }
        private CmsRuntime _cmsRuntime;

        private IApp ContextApp => _app ??= GetBlock().App;
        private IApp _app;

        #region Block

        private ContentBlockBackend Backend => _backend ??= _blockBackendLazy.Value.Init(GetContext(), GetBlock(), Log);
        private ContentBlockBackend _backend;

        /// <summary>
        /// used to be GET Module/GenerateContentBlock
        /// </summary>
        [HttpPost]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public string Block(int parentId, string field, int sortOrder, string app = "", Guid? guid = null)
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
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void Item(int? index = null) => Backend.AddItem(index);

        #endregion


        #region App

        /// <summary>
        /// used to be GET Module/SetAppId
        /// </summary>
        /// <param name="appId"></param>
        [HttpPost]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void App(int? appId)
            => _viewPickerBackendLazy.Value.Init(GetContext(), GetBlock(), Log)
                .SetAppId(appId);

        /// <summary>
        /// used to be GET Module/GetSelectableApps
        /// </summary>
        /// <param name="apps"></param>
        /// <returns></returns>
        [HttpGet]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<AppUiInfo> Apps(string apps = null)
        {
            // Note: we must get the zone-id from the tenant, since the app may not yet exist when inserted the first time
            var tenant = GetContext().Tenant;// new DnnTenant(PortalSettings);
            return _cmsZonesLazy.Value.Init(tenant.ZoneId, Log).AppsRt.GetSelectableApps(tenant, apps)
                .ToList();
        }

        #endregion

        #region Types

        /// <summary>
        /// used to be GET Module/GetSelectableContentTypes
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<ContentTypeUiInfo> ContentTypes() => CmsRuntime?.Views.GetContentTypesWithStatus();

        #endregion

        #region Templates

        /// <summary>
        /// used to be GET Module/GetSelectableTemplates
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<TemplateUiInfo> Templates() => CmsRuntime?.Views.GetCompatibleViews(ContextApp, GetBlock().Configuration);

        /// <summary>
        /// Used in InPage.js
        /// used to be GET Module/SaveTemplateId
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="forceCreateContentGroup"></param>
        /// <returns></returns>
        [HttpPost]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public Guid? Template(int templateId, bool forceCreateContentGroup)
            => _viewPickerBackendLazy.Value.Init(GetContext(), GetBlock(), Log)
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
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public HttpResponseMessage Render(int templateId, string lang)
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
            catch
            {
				//Exceptions.LogException(e);
                throw;
            }
        }

        /// <summary>
        /// Used to be GET Module/Publish
        /// </summary>
        [HttpPost]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public bool Publish(string part, int index) => Backend.PublishPart(part, index);

    }
}