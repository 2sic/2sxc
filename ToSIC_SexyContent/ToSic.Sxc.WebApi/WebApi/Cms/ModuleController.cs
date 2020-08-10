using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Controllers;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Web.Api;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Ui;
using ToSic.Eav.Run;
using ToSic.Eav.Security.Permissions;
using ToSic.SexyContent.WebApi;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Dnn.Run;
using ToSic.Sxc.Security;

namespace ToSic.Sxc.WebApi.Cms
{
    [ValidateAntiForgeryToken]
    // cannot use this, as most requests now come from a lone page [SupportedModules("2sxc,2sxc-app")]
    public partial class ModuleController : SxcApiController
    {
        protected CmsRuntime CmsRuntime => _cmsRuntime ?? (_cmsRuntime = GetCmsRuntime());
        private CmsRuntime _cmsRuntime;

        protected CmsManager CmsManager => _cmsManager ?? (_cmsManager = new CmsManager(App, Log));
        private CmsManager _cmsManager;

        private CmsRuntime GetCmsRuntime()
            // todo: this must be changed, set showDrafts to true for now, as it's probably only used in the view-picker, but it shouldn't just be here
            => App == null ? null : new CmsRuntime(App, Log, true, false);


        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext); // very important!!!
            Log.Rename("2sModC");
            BlockEditor = BlockBuilder.Block.Editor;
        }

        private BlockEditorBase BlockEditor { get; set;  }


        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public Guid? SaveTemplateId(int templateId, bool forceCreateContentGroup)
        {
            var permCheck = new MultiPermissionsApp(BlockBuilder, App.AppId, Log);
            if(!permCheck.EnsureAll(GrantSets.WriteSomething, out var exp))
                throw exp;

            return BlockEditor.SaveTemplateId(templateId, forceCreateContentGroup);
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void SetAppId(int? appId) => BlockEditor.SetAppId(appId);

        #region Get Apps, ContentTypes and Views for UI

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<AppUiInfo> GetSelectableApps(string apps = null)
        {
            // we must get the zone-id from the environment,
            // since the app may not yet exist when inserted the first time
            var tenant = Eav.Factory.Resolve<ITenant>();
            var tenantZoneId = Env.ZoneMapper.GetZoneId(tenant);
            var list = new CmsZones(tenantZoneId, Env, Log).AppsRt.GetSelectableApps(tenant).ToList();

            if (string.IsNullOrWhiteSpace(apps)) return list;

            // New feature in 10.27 - if app-list is provided, only return these
            var appNames = apps.Split(',')
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();
            list = list.Where(ap => appNames
                    .Any(name => string.Equals(name, ap.Name, StringComparison.InvariantCultureIgnoreCase)))
                .ToList();
            return list;
        }

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<ContentTypeUiInfo> GetSelectableContentTypes() => CmsRuntime?.Views.GetContentTypesWithStatus();


        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public IEnumerable<TemplateUiInfo> GetSelectableTemplates() => CmsRuntime?.Views.GetCompatibleViews(App, BlockBuilder.Block.Configuration);

        #endregion



        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public HttpResponseMessage RenderTemplate([FromUri] int templateId, [FromUri] string lang, bool cbIsEntity = false)
        {
            Log.Add($"render template:{templateId}, lang:{lang}, isEnt:{cbIsEntity}");
            try
            {
                // Try setting thread language to enable 2sxc to render the template in this language
                if (!string.IsNullOrEmpty(lang))
                    try
                    {
                        var culture = global::System.Globalization.CultureInfo.GetCultureInfo(lang);
                        global::System.Threading.Thread.CurrentThread.CurrentCulture = culture;
                    }
                    // Fallback / ignore if the language specified has not been found
                    catch (global::System.Globalization.CultureNotFoundException) { }

                var cbToRender = BlockBuilder.Block;

                // if a real templateId was specified, swap to that
                if (templateId > 0)
                {
                    var template = new CmsRuntime(cbToRender.App, Log, Edit.Enabled, false).Views.Get(templateId);
                    ((BlockBuilder)cbToRender.BlockBuilder).View = template;
                }

                var rendered = cbToRender.BlockBuilder.Render().ToString();

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
        {
            Log.Add($"try to publish id #{id}");
            if (!new MultiPermissionsApp(BlockBuilder, App.AppId, Log).EnsureAll(GrantSets.WritePublished, out var exp))
                throw exp;
            new AppManager(App, Log).Entities.Publish(id);
            return true;
        }
    }
}