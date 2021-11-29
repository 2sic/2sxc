using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps.Ui;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Security.Permissions;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks.Edit;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.WebApi.InPage
{
    public class AppViewPickerBackend: BlockWebApiBackendBase<AppViewPickerBackend>
    {

        public AppViewPickerBackend(IServiceProvider sp, Lazy<CmsManager> cmsManagerLazy, IContextResolver ctxResolver) : base(sp, cmsManagerLazy, ctxResolver,"Bck.ViwApp")
        {
        }

        public void SetAppId(int? appId) => BlockEditorBase.GetEditor(Block).SetAppId(appId);

        public IEnumerable<TemplateUiInfo> Templates() =>
            Block?.App == null 
                ? new TemplateUiInfo[0] 
                : CmsManagerOfBlock?.Read.Views.GetCompatibleViews(Block?.App, Block?.Configuration);

        public IEnumerable<AppUiInfo> Apps(string apps = null)
        {
            // Note: we must get the zone-id from the tenant, since the app may not yet exist when inserted the first time
            var tenant = ContextOfBlock.Site;
            return ServiceProvider.Build<CmsZones>().Init(tenant.ZoneId, Log)
                .AppsRt
                .GetSelectableApps(tenant, apps)
                .ToList();
        }

        public IEnumerable<ContentTypeUiInfo> ContentTypes()
        {
            // nothing to do without app
            if (Block?.App == null) return null;

            var appPath = Block.App.Path ?? "";
            return CmsManagerOfBlock?.Read.Views.GetContentTypesWithStatus(appPath);
        }

        public Guid? SaveTemplateId(int templateId, bool forceCreateContentGroup)
        {
            var callLog = Log.Call<Guid?>($"{templateId}, {forceCreateContentGroup}");
            ThrowIfNotAllowedInApp(GrantSets.WriteSomething);
            return callLog("ok", BlockEditorBase.GetEditor(Block).SaveTemplateId(templateId, forceCreateContentGroup));
        }

        public bool Publish(int id)
        {
            var callLog = Log.Call<bool>($"{id}");
            ThrowIfNotAllowedInApp(GrantSets.WritePublished);
            CmsManagerOfBlock.Entities.Publish(id);
            return callLog("ok", true);
        }

    }
}
