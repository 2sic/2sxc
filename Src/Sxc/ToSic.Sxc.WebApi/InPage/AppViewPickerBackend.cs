using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Apps.Ui;
using ToSic.Lib.Logging;
using ToSic.Eav.Security.Permissions;
using ToSic.Lib.DI;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks.Edit;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.WebApi.InPage
{
    public class AppViewPickerBackend: BlockWebApiBackendBase<AppViewPickerBackend>
    {
        public AppViewPickerBackend(Generator<MultiPermissionsApp> multiPermissionsApp, 
            LazyInit<CmsManager> cmsManagerLazy, 
            IContextResolver ctxResolver, 
            Generator<BlockEditorForModule> blkEdtForMod,
            Generator<BlockEditorForEntity> blkEdtForEnt
            ) : base(multiPermissionsApp, cmsManagerLazy, ctxResolver,"Bck.ViwApp")
        {
            ConnectServices(
                _blkEdtForMod = blkEdtForMod,
                _blkEdtForEnt = blkEdtForEnt
            );
        }

        private readonly IGenerator<BlockEditorForModule> _blkEdtForMod;
        private readonly IGenerator<BlockEditorForEntity> _blkEdtForEnt;

        public void SetAppId(int? appId) => BlockEditorBase.GetEditor(Block, _blkEdtForMod, _blkEdtForEnt).SetAppId(appId);

        public IEnumerable<TemplateUiInfo> Templates() =>
            Block?.App == null 
                ? Array.Empty<TemplateUiInfo>()
                : CmsManagerOfBlock?.Read.Views.GetCompatibleViews(Block?.App, Block?.Configuration);

        // 2022-12-20 2dm - doesn't seem to be in use?
        //public IEnumerable<AppUiInfo> Apps(string apps = null)
        //{
        //    // Note: we must get the zone-id from the tenant, since the app may not yet exist when inserted the first time
        //    var tenant = ContextOfBlock.Site;
        //    return GetService<CmsZones>().Init(Log).SetId(tenant.ZoneId)
        //        .AppsRt
        //        .GetSelectableApps(tenant, apps)
        //        .ToList();
        //}

        public IEnumerable<ContentTypeUiInfo> ContentTypes()
        {
            // nothing to do without app
            if (Block?.App == null) return null;
            return CmsManagerOfBlock?.Read.Views.GetContentTypesWithStatus(Block.App.Path ?? "", Block.App.PathShared ?? "");
        }

        public Guid? SaveTemplateId(int templateId, bool forceCreateContentGroup)
        {
            var callLog = Log.Fn<Guid?>($"{templateId}, {forceCreateContentGroup}");
            ThrowIfNotAllowedInApp(GrantSets.WriteSomething);
            return callLog.ReturnAsOk(BlockEditorBase.GetEditor(Block, _blkEdtForMod, _blkEdtForEnt).SaveTemplateId(templateId, forceCreateContentGroup));
        }

        public bool Publish(int id)
        {
            var callLog = Log.Fn<bool>($"{id}");
            ThrowIfNotAllowedInApp(GrantSets.WritePublished);
            CmsManagerOfBlock.Entities.Publish(id);
            return callLog.ReturnTrue("ok");
        }
    }
}
