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
    public class AppViewPickerBackend: BlockWebApiBackendBase
    {
        private readonly LazySvc<BlockEditorSelector> _blockEditorSelectorLazy;

        public AppViewPickerBackend(
            Generator<MultiPermissionsApp> multiPermissionsApp,
            LazySvc<CmsManager> cmsManagerLazy, 
            IContextResolver ctxResolver,
            LazySvc<BlockEditorSelector> blockEditorSelectorLazy,
            LazySvc<AppWorkSxc> appSysSxc
            ) : base(multiPermissionsApp, cmsManagerLazy, appSysSxc, ctxResolver,"Bck.ViwApp")
        {
            ConnectServices(
                _blockEditorSelectorLazy = blockEditorSelectorLazy
            );
        }

        public void SetAppId(int? appId) => _blockEditorSelectorLazy.Value.GetEditor(Block).SetAppId(appId);

        public IEnumerable<TemplateUiInfo> Templates() =>
            Block?.App == null 
                ? Array.Empty<TemplateUiInfo>()
                : AppSysSxc.Value.AppViews(AppWorkCtx) /*CmsManagerOfBlock?.Read.Views*/.GetCompatibleViews(Block?.App, Block?.Configuration);

        public IEnumerable<ContentTypeUiInfo> ContentTypes()
        {
            // nothing to do without app
            if (Block?.App == null) return null;
            return AppSysSxc.Value.AppViews(AppWorkCtx) /*CmsManagerOfBlock?.Read.Views*/.GetContentTypesWithStatus(Block.App.Path ?? "", Block.App.PathShared ?? "");
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
            CmsManagerOfBlock.Entities.Publish(id);
            return callLog.ReturnTrue("ok");
        }
    }
}
