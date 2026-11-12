using ToSic.Eav.Apps.Inspect.Sys.UiDto;
using ToSic.Sxc.Blocks.Sys.BlockEditor;
using ToSic.Sxc.Blocks.Sys.Work;
using ToSic.Sys.Security.Permissions;

namespace ToSic.Sxc.Backend.InPage;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class AppViewPickerBackend(
    PermissionCheckAction permCheckAction,
    IContextCompleteService ctxService,
    Generator<BlockUpdateTemplate> blockUpdateTemplate,
    AppWorkChain<WorkBlockViewsGet> workBlockViews,
    AppWorkContextService appWorkCtxService,
    LazySvc<WorkViewsContentTypes> workViewsContentTypes)
    : ServiceBase("Bck.ViwApp", connect: [permCheckAction, ctxService, blockUpdateTemplate, workBlockViews, appWorkCtxService, workViewsContentTypes])
{
    public IEnumerable<TemplateUiInfo> Templates()
    {
        var block = ctxService.BlockRequired();
        return block.AppOrNull == null
            ? []
            : workBlockViews.New(appWorkCtxService.ContextNew(block.Context.AppReaderRequired))
                .GetCompatibleViews(block);
    }

    public IEnumerable<ContentTypeUiInfo> ContentTypes()
    {
        var block = ctxService.BlockRequired();
        return block.AppOrNull is not {} app
            ? []
            : workViewsContentTypes.Value.GetContentTypesWithStatus(
                appWorkCtxService.ContextNew(block.Context.AppReaderRequired),
                app.AppPaths.Path ?? "",
                app.AppPaths.PathShared ?? ""
            );
    }

    public async Task<Guid?> SaveTemplateId(int templateId, bool forceCreateContentGroup)
    {
        var l = Log.Fn<Guid?>($"{templateId}, {forceCreateContentGroup}");
        var block = ctxService.BlockRequired();
        await permCheckAction.Handle(new(), new((block.Context, GrantSets.WriteSomething)));
        var result = await blockUpdateTemplate.New()
            .Handle(new(), new((block, templateId, forceCreateContentGroup)));
        return l.Return(result.Data);
    }
}