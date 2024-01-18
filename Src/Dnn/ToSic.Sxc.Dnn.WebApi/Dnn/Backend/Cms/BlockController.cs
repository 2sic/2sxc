using ToSic.Eav.Apps.Internal.Ui;
using ToSic.Sxc.Backend.Cms;
using ToSic.Sxc.Backend.InPage;
using RealController = ToSic.Sxc.Backend.Cms.BlockControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Cms;

[ValidateAntiForgeryToken]
// cannot use this, as most requests now come from a lone page [SupportedModules(DnnSupportedModuleNames)]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class BlockController() : DnnSxcControllerBase(RealController.LogSuffix), IBlockController
{
    private RealController Real => SysHlp.GetService<RealController>();

    /// <inheritdoc />
    [HttpPost]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public string Block(int parentId, string field, int index, string app = "", Guid? guid = null)
        => Real.Block(parentId, field, index, app, guid);


    /// <inheritdoc />
    [HttpPost]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public void Item([FromUri] int? index = null) => Real.Item(index);


    /// <inheritdoc />
    [HttpPost]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public void App(int? appId) => Real.App(appId);

    /// <inheritdoc />
    [HttpGet]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public IEnumerable<AppUiInfo> Apps(string apps = null) => Real.Apps(apps);


    /// <inheritdoc />
    [HttpGet]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public IEnumerable<ContentTypeUiInfo> ContentTypes() => Real.ContentTypes();


    /// <inheritdoc />
    [HttpGet]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public IEnumerable<TemplateUiInfo> Templates() => Real.Templates();

    /// <inheritdoc />
    [HttpPost]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    public Guid? Template(int templateId, bool forceCreateContentGroup) => Real.Template(templateId, forceCreateContentGroup);


    /// <inheritdoc />
    [HttpGet]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public AjaxRenderDto Render([FromUri] int templateId, [FromUri] string lang, [FromUri] string edition) 
        => Real.Set(DnnConstants.SysFolderRootVirtual.Trim('~')).Render(templateId, lang, edition);

    /// <inheritdoc />
    [HttpPost]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public bool Publish(string part, int index) => Real.Publish(part, index);
}