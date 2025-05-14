using ToSic.Eav.Apps.Internal.Ui;
using ToSic.Eav.Security.Internal;
using ToSic.Sxc.Apps.Internal.Work;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Blocks.Work.Internal;

namespace ToSic.Sxc.Backend.InPage;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class AppViewPickerBackend(
    Generator<MultiPermissionsApp> multiPermissionsApp,
    ISxcContextResolver ctxResolver,
    LazySvc<BlockEditorSelector> blockEditorSelectorLazy,
    GenWorkPlus<WorkViews> workViews,
    GenWorkPlus<WorkBlockViewsGet> workBlockViews,
    AppWorkContextService appWorkCtxService,
    GenWorkDb<WorkEntityPublish> publisher)
    : BlockWebApiBackendBase(multiPermissionsApp, appWorkCtxService, ctxResolver, "Bck.ViwApp",
        connect: [workViews, publisher, blockEditorSelectorLazy])
{
    public void SetAppId(int? appId) => blockEditorSelectorLazy.Value.GetEditor(Block).SetAppId(appId);

    public IEnumerable<TemplateUiInfo> Templates() =>
        Block?.App == null 
            ? Array.Empty<TemplateUiInfo>()
            : workBlockViews.New(AppWorkCtxPlus)
                .GetCompatibleViews(Block?.App, Block?.Configuration);
        //: workViews.New(AppWorkCtxPlus).GetCompatibleViews(Block?.App, Block?.Configuration);

    public IEnumerable<ContentTypeUiInfo> ContentTypes()
    {
        // nothing to do without app
        if (Block?.App == null) return null;
        return workViews.New(AppWorkCtxPlus).GetContentTypesWithStatus(Block.App.Path ?? "", Block.App.PathShared ?? "");
    }

    public Guid? SaveTemplateId(int templateId, bool forceCreateContentGroup)
    {
        var callLog = Log.Fn<Guid?>($"{templateId}, {forceCreateContentGroup}");
        ThrowIfNotAllowedInApp(GrantSets.WriteSomething);
        return callLog.ReturnAsOk(blockEditorSelectorLazy.Value.GetEditor(Block).SaveTemplateId(templateId, forceCreateContentGroup));
    }

    public bool Publish(int id)
    {
        var callLog = Log.Fn<bool>($"{id}");
        ThrowIfNotAllowedInApp(GrantSets.WritePublished);
        publisher.New(AppWorkCtx).Publish(id);
        return callLog.ReturnTrue("ok");
    }
}