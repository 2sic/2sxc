using ToSic.Eav.Apps.Internal.Ui;
using ToSic.Eav.Apps.Sys.Permissions;
using ToSic.Sxc.Apps.Internal.Work;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Blocks.Work.Internal;
using ToSic.Sys.Security.Permissions;

namespace ToSic.Sxc.Backend.InPage;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class AppViewPickerBackend(
    Generator<MultiPermissionsApp> multiPermissionsApp,
    ISxcCurrentContextService ctxService,
    LazySvc<BlockEditorSelector> blockEditorSelectorLazy,
    GenWorkPlus<WorkViews> workViews,
    GenWorkPlus<WorkBlockViewsGet> workBlockViews,
    AppWorkContextService appWorkCtxService,
    GenWorkDb<WorkEntityPublish> publisher)
    : BlockWebApiBackendBase(multiPermissionsApp, appWorkCtxService, ctxService, "Bck.ViwApp",
        connect: [workViews, publisher, blockEditorSelectorLazy])
{
    public void SetAppId(int? appId) => blockEditorSelectorLazy.Value.GetEditor(Block).SetAppId(appId);

    public IEnumerable<TemplateUiInfo> Templates() =>
        Block?.App == null 
            ? Array.Empty<TemplateUiInfo>()
            : workBlockViews.New(AppWorkCtxPlus).GetCompatibleViews(Block);

    public IEnumerable<ContentTypeUiInfo> ContentTypes() => Block?.App == null
        ? null
        : workViews.New(AppWorkCtxPlus).GetContentTypesWithStatus(Block.App.Path ?? "", Block.App.PathShared ?? "");

    public Guid? SaveTemplateId(int templateId, bool forceCreateContentGroup)
    {
        var l = Log.Fn<Guid?>($"{templateId}, {forceCreateContentGroup}");
        ThrowIfNotAllowedInApp(GrantSets.WriteSomething);
        return l.ReturnAsOk(blockEditorSelectorLazy.Value.GetEditor(Block).SaveTemplateId(templateId, forceCreateContentGroup));
    }

    public bool Publish(int id)
    {
        var l = Log.Fn<bool>($"{id}");
        ThrowIfNotAllowedInApp(GrantSets.WritePublished);
        publisher.New(AppWorkCtx).Publish(id);
        return l.ReturnTrue("ok");
    }
}