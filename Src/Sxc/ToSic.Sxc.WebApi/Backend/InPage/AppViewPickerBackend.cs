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
    IContextCompleteService ctxService,
    Generator<BlockUpdateApp> blockUpdateApp,
    Generator<BlockUpdateTemplate> blockUpdateTemplate,
    AppWorkChain<WorkBlockViewsGet> workBlockViews,
    AppWorkContextService appWorkCtxService,
    LazySvc<WorkEntityPublish> workPublish,
    LazySvc<WorkViewsContentTypes> workViewsContentTypes)
    : ServiceBase("Bck.ViwApp", connect: [multiPermissionsApp, ctxService, blockUpdateApp, blockUpdateTemplate, workBlockViews, appWorkCtxService, workPublish, workViewsContentTypes])
{
    public void SetAppId(int? appId)
        => blockUpdateApp.New().SetAppId(ctxService.BlockRequired(), appId);
    
    private IBlock Block => ctxService.BlockRequired();
    private IAppWorkContext AppCtx => field ??= appWorkCtxService.ContextNew(Block.Context.AppReaderRequired);

    public IEnumerable<TemplateUiInfo> Templates()
    {
        var block = Block;
        return block.AppOrNull == null
            ? []
            : workBlockViews.New(AppCtx)
                .GetCompatibleViews(block);
    }

    public IEnumerable<ContentTypeUiInfo> ContentTypes()
    {
        var block = Block;
        return block.AppOrNull is not {} app
            ? []
            : workViewsContentTypes.Value.GetContentTypesWithStatus(
                AppCtx,
                app.AppPaths.Path ?? "",
                app.AppPaths.PathShared ?? ""
            );
    }

    public Guid? SaveTemplateId(int templateId, bool forceCreateContentGroup)
    {
        var l = Log.Fn<Guid?>($"{templateId}, {forceCreateContentGroup}");
        multiPermissionsApp.ThrowIfNotAllowedInApp(Block.Context, GrantSets.WriteSomething);
        var result = blockUpdateTemplate.New()
            .SaveTemplateId(Block, templateId, forceCreateContentGroup);
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