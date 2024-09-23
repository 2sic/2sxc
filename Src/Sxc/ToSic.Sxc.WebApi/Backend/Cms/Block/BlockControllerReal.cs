using ToSic.Eav.Apps.Internal.Ui;
using ToSic.Sxc.Apps.Internal.Work;
using ToSic.Sxc.Backend.ContentBlocks;
using ToSic.Sxc.Backend.InPage;
using ServiceBase = ToSic.Lib.Services.ServiceBase;

namespace ToSic.Sxc.Backend.Cms;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class BlockControllerReal(
    LazySvc<IContextOfSite> context,
    LazySvc<ContentBlockBackend> blockBackend,
    LazySvc<AppViewPickerBackend> viewsBackend,
    LazySvc<WorkApps> workApps)
    : ServiceBase($"{Eav.EavLogs.WebApi}.{LogSuffix}Rl", connect: [context, blockBackend, viewsBackend, workApps]),
        IBlockController
{
    public const string LogSuffix = "Block";


    #region Block

    private ContentBlockBackend Backend => _backend = _backend ?? blockBackend.Value;
    private ContentBlockBackend _backend;

    /// <inheritdoc />
    public string Block(int parentId, string field, int index, string app = "", Guid? guid = null)
        => Backend.NewBlockAndRender(parentId, field, index, app, guid).Html;
    #endregion

    #region BlockItems
    /// <summary>
    /// used to be GET Module/AddItem
    /// </summary>
    public void Item(int? index = null)
    {
        Backend.AddItem(index);
    }

    #endregion


    #region App

    /// <summary>
    /// used to be GET Module/SetAppId
    /// </summary>
    /// <param name="appId"></param>

    public void App(int? appId) => viewsBackend.Value.SetAppId(appId);

    /// <summary>
    /// used to be GET Module/GetSelectableApps
    /// </summary>
    /// <param name="apps"></param>
    /// <returns></returns>
    public IEnumerable<AppUiInfo> Apps(string apps = null)
    {
        // Note: we must get the zone-id from the tenant, since the app may not yet exist when inserted the first time
        var site = context.Value.Site;
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
    public Guid? Template(int templateId, bool forceCreateContentGroup)
        => viewsBackend.Value
            .SaveTemplateId(templateId, forceCreateContentGroup);

    #endregion

    /// <inheritdoc />
    public AjaxRenderDto Render(int templateId, string lang, string edition)
    {
        Log.A($"render template:{templateId}, lang:{lang}");
        return Backend.RenderForAjax(templateId, lang, _moduleRoot, edition);
    }
    public BlockControllerReal Set(string moduleRoot)
    {
        _moduleRoot = moduleRoot;
        return this;
    }
    private string _moduleRoot;


    /// <inheritdoc />
    public bool Publish(string part, int index) => Backend.PublishPart(part, index);
}