using System.Web.Http.Controllers;
using ToSic.Eav.Apps.Inspect.Sys.UiDto;
using ToSic.Eav.WebApi.Sys;
using ToSic.Sxc.Backend.Cms;
using ToSic.Sxc.Backend.InPage;
using ToSic.Sxc.Dnn.WebApi.Sys;
using ToSic.Sxc.Dnn.WebApi.Sys.HttpJson;
using RealController = ToSic.Sxc.Backend.Cms.BlockControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Cms;

[DnnLogWebApi, DnnLogExceptions, ConfigureJsonOnlyResponse] // #CompositionOverInheritance
[ValidateAntiForgeryToken]
// cannot use this, as most requests now come from a lone page [SupportedModules(DnnSupportedModuleNames)]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class BlockController : DnnApiController, IBlockController
{
    #region WebApi Composition Infrastructure

    public ILog Log => field ??= LogFactory.Create($"Api.{RealController.LogSuffix}");

    private DnnWebApiHelper SysHlp => field ??= new(this, EavWebApiConstants.HistoryNameWebApi);

    /// <summary>
    /// The block controller needs this for every request.
    /// </summary>
    /// <param name="controllerContext"></param>
    protected override void Initialize(HttpControllerContext controllerContext)
    {
        base.Initialize(controllerContext);
        new DnnSxcWebApiInitBlockContextAndShare().InitBlockSpecs(SysHlp, controllerContext.Request, Log);
    }

    #endregion


    private RealController Real => SysHlp.GetService<RealController>();

    /// <inheritdoc />
    [HttpPost]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public string Block(int parentId, string field, int index, string app = "", Guid? guid = null)
        => Real.Block(parentId, field, index, app, guid);


    /// <inheritdoc />
    [HttpPost]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public void Item([FromUri] int? index = null)
        => Real.Item(index);


    /// <inheritdoc />
    [HttpPost]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public Task App(int? appId)
        => Real.App(appId);

    /// <inheritdoc />
    [HttpGet]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public IEnumerable<AppUiInfo> Apps(string apps = null)
        => Real.Apps(apps);


    /// <inheritdoc />
    [HttpGet]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public IEnumerable<ContentTypeUiInfo> ContentTypes()
        => Real.ContentTypes();


    /// <inheritdoc />
    [HttpGet]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public IEnumerable<TemplateUiInfo> Templates()
        => Real.Templates();

    /// <inheritdoc />
    [HttpPost]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    public Task<Guid?> Template(int templateId, bool forceCreateContentGroup)
        => Real.Template(templateId, forceCreateContentGroup);


    /// <inheritdoc />
    [HttpGet]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public AjaxRenderDto Render([FromUri] int templateId, [FromUri] string lang, [FromUri] string edition) 
        => Real.Set(SysFolderRootVirtual.Trim('~')).Render(templateId, lang, edition);

    /// <inheritdoc />
    [HttpPost]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public async Task<bool> Publish(string part, int index)
        => await Real.Publish(part, index);
}