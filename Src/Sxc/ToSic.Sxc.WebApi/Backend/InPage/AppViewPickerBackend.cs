using ToSic.Eav.Apps.Sys.Permissions;
using ToSic.Sxc.Apps.Sys.Ui;
using ToSic.Sxc.Blocks.Sys.BlockEditor;
using ToSic.Sxc.Blocks.Sys.Work;
using ToSic.Sys.Security.Permissions;

namespace ToSic.Sxc.Backend.InPage;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class AppViewPickerBackend(
    Generator<MultiPermissionsApp, MultiPermissionsApp.Options> multiPermissionsApp,
    ISxcCurrentContextService ctxService,
    LazySvc<BlockEditorSelector> blockEditorSelectorLazy,
    GenWorkPlus<WorkViews> workViews,
    GenWorkPlus<WorkBlockViewsGet> workBlockViews,
    AppWorkContextService appWorkCtxService,
    LazySvc<WorkEntityPublish> workPublish)
    : ServiceBase("Bck.ViwApp", connect: [multiPermissionsApp, ctxService, blockEditorSelectorLazy, workViews, workBlockViews, appWorkCtxService, workPublish])
{
    public void SetAppId(int? appId)
        => blockEditorSelectorLazy.Value
            .GetEditor(ctxService.BlockRequired())
            .SetAppId(appId);

    public IEnumerable<TemplateUiInfo> Templates()
    {
        var block = ctxService.BlockRequired();
        return workBlockViews.New(appWorkCtxService.ContextPlus(block.Context.AppReaderRequired))
                .GetCompatibleViews(block);
    }

    public IEnumerable<ContentTypeUiInfo> ContentTypes()
    {
        var block = ctxService.BlockRequired();
        return workViews.New(appWorkCtxService.ContextPlus(block.Context.AppReaderRequired))
                .GetContentTypesWithStatus(block.App.Path ?? "", block.App.PathShared ?? "");
    }

    public Guid? SaveTemplateId(int templateId, bool forceCreateContentGroup)
    {
        var l = Log.Fn<Guid?>($"{templateId}, {forceCreateContentGroup}");
        var block = ctxService.BlockRequired();
        multiPermissionsApp.ThrowIfNotAllowedInApp(block.Context, GrantSets.WriteSomething);
        var result = blockEditorSelectorLazy.Value.GetEditor(block)
            .SaveTemplateId(templateId, forceCreateContentGroup);
        return l.ReturnAsOk(result);
    }

    public bool Publish(int id)
    {
        var l = Log.Fn<bool>($"{id}");
        var block = ctxService.BlockRequired();
        multiPermissionsApp.ThrowIfNotAllowedInApp(block.Context, GrantSets.WritePublished);
        using (appWorkCtxService.WithContext(block.Context.AppReaderRequired))
            workPublish.Value.Publish([id]);
        return l.ReturnTrue("ok");
    }
}