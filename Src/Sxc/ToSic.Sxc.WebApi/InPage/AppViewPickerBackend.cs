using System;
using System.Collections.Generic;
using ToSic.Eav.Apps.AppSys;
using ToSic.Eav.Apps.Security;
using ToSic.Eav.Apps.Ui;
using ToSic.Eav.Apps.Work;
using ToSic.Lib.Logging;
using ToSic.Eav.Security.Permissions;
using ToSic.Lib.DI;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks.Edit;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.WebApi.InPage
{
    public class AppViewPickerBackend: BlockWebApiBackendBase
    {
        public AppViewPickerBackend(
            Generator<MultiPermissionsApp> multiPermissionsApp,
            LazySvc<CmsManager> cmsManagerLazy,
            IContextResolver ctxResolver,
            LazySvc<BlockEditorSelector> blockEditorSelectorLazy,
            LazySvc<AppWorkSxc> appSysSxc,
            LazySvc<AppWorkUnit<WorkEntityPublish, IAppWorkCtxWithDb>> publisher
            ) : base(multiPermissionsApp, cmsManagerLazy, appSysSxc, ctxResolver,"Bck.ViwApp")
        {
            _publisher = publisher;
            ConnectServices(
                _blockEditorSelectorLazy = blockEditorSelectorLazy
            );
        }

        private readonly LazySvc<AppWorkUnit<WorkEntityPublish, IAppWorkCtxWithDb>> _publisher;
        private readonly LazySvc<BlockEditorSelector> _blockEditorSelectorLazy;

        public void SetAppId(int? appId) => _blockEditorSelectorLazy.Value.GetEditor(Block).SetAppId(appId);

        public IEnumerable<TemplateUiInfo> Templates() =>
            Block?.App == null 
                ? Array.Empty<TemplateUiInfo>()
                : AppSysSxc.Value.AppViews(AppWorkCtxPlus).GetCompatibleViews(Block?.App, Block?.Configuration);

        public IEnumerable<ContentTypeUiInfo> ContentTypes()
        {
            // nothing to do without app
            if (Block?.App == null) return null;
            return AppSysSxc.Value.AppViews(AppWorkCtxPlus).GetContentTypesWithStatus(Block.App.Path ?? "", Block.App.PathShared ?? "");
        }

        public Guid? SaveTemplateId(int templateId, bool forceCreateContentGroup)
        {
            var callLog = Log.Fn<Guid?>($"{templateId}, {forceCreateContentGroup}");
            ThrowIfNotAllowedInApp(GrantSets.WriteSomething);
            return callLog.ReturnAsOk(_blockEditorSelectorLazy.Value.GetEditor(Block).SaveTemplateId(templateId, forceCreateContentGroup));
        }

        public bool Publish(int id)
        {
            var callLog = Log.Fn<bool>($"{id}");
            ThrowIfNotAllowedInApp(GrantSets.WritePublished);
            _publisher.Value.New(AppWorkCtx).Publish(id);
            return callLog.ReturnTrue("ok");
        }
    }
}
