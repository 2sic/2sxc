using ToSic.Eav.Apps.Sys.Permissions;
using ToSic.Sxc.Apps.Sys.Ui;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Blocks.Sys.BlockEditor;
using ToSic.Sxc.Blocks.Sys.Work;
using ToSic.Sys.Security.Permissions;

namespace ToSic.Sxc.Backend.InPage;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class AppViewPickerBackend(
    Generator<MultiPermissionsApp, MultiPermissionsApp.Options> multiPermissionsApp,
    ISxcCurrentContextService ctxService,
    LazySvc<BlockEditorSelector> blockEditorSelectorLazy,
    AppWorkChain<WorkBlockViewsGet> workBlockViews,
    AppWorkContextService appWorkCtxService,
    LazySvc<WorkEntityPublish> workPublish,
    LazySvc<WorkViewsContentTypes> workViewsContentTypes)
    : ServiceBase("Bck.ViwApp", connect: [multiPermissionsApp, ctxService, blockEditorSelectorLazy, workBlockViews, appWorkCtxService, workPublish, workViewsContentTypes])
{
    public void SetAppId(int? appId)
        => blockEditorSelectorLazy.Value
            .GetEditor(ctxService.BlockRequired())
            .SetAppId(appId);
    
    private IBlock Block => ctxService.BlockRequired();
    private IAppWorkContext AppCtx => field ??= appWorkCtxService.ContextNew(Block.Context.AppReaderRequired);

    public IEnumerable<TemplateUiInfo> Templates()
    {
        return workBlockViews.New(AppCtx)
                .GetCompatibleViews(Block);
    }

    public IEnumerable<ContentTypeUiInfo> ContentTypes()
    {
        return workViewsContentTypes.Value.GetContentTypesWithStatus(
            AppCtx,
            Block.App.Path ?? "",
            Block.App.PathShared ?? ""
        );
    }

    public Guid? SaveTemplateId(int templateId, bool forceCreateContentGroup)
    {
        var l = Log.Fn<Guid?>($"{templateId}, {forceCreateContentGroup}");
        multiPermissionsApp.ThrowIfNotAllowedInApp(Block.Context, GrantSets.WriteSomething);
        var result = blockEditorSelectorLazy.Value.GetEditor(Block)
            .SaveTemplateId(templateId, forceCreateContentGroup);
        return l.ReturnAsOk(result);
    }

    public bool Publish(int id)
    {
        var l = Log.Fn<bool>($"{id}");
        multiPermissionsApp.ThrowIfNotAllowedInApp(Block.Context, GrantSets.WritePublished);
        workPublish.Value.Publish(AppCtx, [id]);
        return l.ReturnTrue("ok");
    }
}