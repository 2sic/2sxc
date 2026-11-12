using ToSic.Eav.Apps.Inspect.Sys.UiDto;
using ToSic.Eav.Sys;
using ToSic.Sxc.Backend.ContentBlocks;
using ToSic.Sxc.Backend.InPage;
using ToSic.Sxc.Blocks.Sys.BlockEditor;
using ToSic.Sys.HookUp;

namespace ToSic.Sxc.Backend.Cms;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class BlockControllerReal(
    LazySvc<IContextOfSiteService> ctxSvc,
    LazySvc<ContentBlockBackend> blockBackend,
    LazySvc<AppViewPickerBackend> viewsBackend,
    LazySvc<PublishWebApi> publish,
    LazySvc<WorkApps> workApps,
    LazySvc<IContextCompleteService> ctxService,
    Generator<BlockUpdateApp> blockUpdateApp)
    : ServiceBase($"{EavLogs.WebApi}.{LogSuffix}Rl", connect: [ctxSvc, blockBackend, viewsBackend, publish, workApps, ctxService, blockUpdateApp]),
        IBlockController
{
    public const string LogSuffix = "Block";


    #region Block

    [field: AllowNull, MaybeNull]
    private ContentBlockBackend Backend => field ??= blockBackend.Value;

    /// <inheritdoc />
    public string Block(int parentId, string field, int index, string app = "", Guid? guid = null)
        => Backend.NewBlockAndRender(parentId, field, index, app, guid).Html ?? "";
    #endregion

    #region BlockItems
    /// <summary>
    /// used to be GET Module/AddItem
    /// </summary>
    public void Item(int? index = null)
        => Backend.AddItem(index);

    #endregion


    #region App

    /// <summary>
    /// used to be GET Module/SetAppId
    /// </summary>
    /// <param name="appId"></param>

    public Task App(int? appId)
        => blockUpdateApp.New().Handle(new(), (ctxService.Value.BlockRequired(), appId).ToPackage());

    /// <summary>
    /// used to be GET Module/GetSelectableApps
    /// </summary>
    /// <param name="apps"></param>
    /// <returns></returns>
    public IEnumerable<AppUiInfo> Apps(string? apps = null)
    {
        // Note: we must get the zone-id from the tenant, since the app may not yet exist when inserted the first time
        var site = ctxSvc.Value.Context.Site;
        return [.. workApps.Value.GetSelectableApps(site, apps)];
    }

    #endregion

    #region Types

    /// <inheritdoc />
    public IEnumerable<ContentTypeUiInfo> ContentTypes() => viewsBackend.Value.ContentTypes();

    #endregion

    #region Templates

    /// <summary>
    /// used to be GET Module/GetSelectableTemplates
    /// </summary>
    /// <returns></returns>
    public IEnumerable<TemplateUiInfo> Templates() => viewsBackend.Value.Templates();

    /// <summary>
    /// Used in InPage.js
    /// used to be GET Module/SaveTemplateId
    /// </summary>
    /// <param name="templateId"></param>
    /// <param name="forceCreateContentGroup"></param>
    /// <returns></returns>
    public Task<Guid?> Template(int templateId, bool forceCreateContentGroup)
        => viewsBackend.Value
            .SaveTemplateId(templateId, forceCreateContentGroup);

    #endregion

    /// <inheritdoc />
    public AjaxRenderDto Render(int templateId, string lang, string edition)
        => Backend.RenderForAjax(templateId, lang, _moduleRoot, edition);

    public BlockControllerReal Set(string moduleRoot)
    {
        _moduleRoot = moduleRoot;
        return this;
    }

    private string _moduleRoot = null!;

    /// <inheritdoc />
    public Task<bool> Publish(string part, int index)
        => publish.Value.PublishPart(part, index);
}