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
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.WebApi.InPage;

namespace ToSic.Sxc.WebApi.Cms
{
    [ValidateAntiForgeryToken]
    // cannot use this, as most requests now come from a lone page [SupportedModules("2sxc,2sxc-app")]
    public partial class ModuleController : SxcApiController
    {
        protected override string HistoryLogName => "Api.ModCnt";

        protected CmsRuntime CmsRuntime => _cmsRuntime ?? (_cmsRuntime = App == null ? null : new CmsRuntime(App, Log, true, false));
        private CmsRuntime _cmsRuntime;

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public Guid? SaveTemplateId(int templateId, bool forceCreateContentGroup)
            => new AppViewPickerBackend().Init(GetContext(), GetBlock(), Log)
                .SaveTemplateId(templateId, forceCreateContentGroup);

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void SetAppId(int? appId)
            => new AppViewPickerBackend().Init(GetContext(), GetBlock(), Log)
                .SetAppId(appId);

        #region Get Apps, ContentTypes and Views for UI

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<AppUiInfo> GetSelectableApps(string apps = null)
        {
            // Note: we must get the zone-id from the tenant, since the app may not yet exist when inserted the first time
            var tenant = new DnnTenant(PortalSettings);
            return new CmsZones(tenant.ZoneId, Log).AppsRt.GetSelectableApps(tenant, apps).ToList();
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<ContentTypeUiInfo> GetSelectableContentTypes() => CmsRuntime?.Views.GetContentTypesWithStatus();


        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<TemplateUiInfo> GetSelectableTemplates() => CmsRuntime?.Views.GetCompatibleViews(App, GetBlock().Configuration);

        #endregion



        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public HttpResponseMessage RenderTemplate([FromUri] int templateId, [FromUri] string lang)
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



        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public bool Publish(int id)
            => new AppViewPickerBackend().Init(GetContext(), GetBlock(), Log)
                .Publish(id);
    }
}