using System.Web.Http.Controllers;
using ToSic.Eav.WebApi.Sys;
using ToSic.Eav.WebApi.Sys.Cms;
using ToSic.Sxc.Backend.Cms;
using ToSic.Sxc.Dnn.WebApi.Sys;
using ToSic.Sxc.Dnn.WebApi.Sys.HttpJson;
using RealController = ToSic.Sxc.Backend.Cms.ListControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Cms;

[DnnLogWebApi, DnnLogExceptions, ConfigureJsonOnlyResponse] // #CompositionOverInheritance
[SupportedModules(DnnSupportedModuleNames)]
[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class ListController : DnnApiController, IListController
{
    #region WebApi Composition Infrastructure

    public ILog Log => field ??= LogFactory.Create($"Api.{RealController.LogSuffix}");

    private DnnWebApiHelper SysHlp => field ??= new(this, EavWebApiConstants.HistoryNameWebApi);

    /// <summary>
    /// The app data controller probably needs this for every request.
    /// </summary>
    /// <param name="controllerContext"></param>
    protected override void Initialize(HttpControllerContext controllerContext)
    {
        base.Initialize(controllerContext);
        new DnnSxcWebApiInitBlockContextAndShare().InitBlockSpecs(SysHlp, controllerContext.Request, Log);
    }

    #endregion


    private RealController Real => SysHlp.GetService<RealController>();

    [HttpPost]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public void Replace(Guid parent, string part, int index, int entityId, bool add = false)
        => Real.Replace(parent, part, index, entityId, add);


    [HttpGet]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public ListSlotSwapInfo ReplaceOptions(Guid parent, string part, int index, string? contentType = null)
        => Real.ReplaceOptions(parent, part, index, contentType);

    /// <inheritdoc />
    /// <summary>
    /// used to be GET Module/ChangeOrder
    /// </summary>
    [HttpPost]
    public void Move(Guid? parent, string fields, int index, int toIndex)
        => Real.Move(parent, fields, index, toIndex);


    /// <inheritdoc />
    /// <summary>
    /// Used to be Get Module/RemoveFromList
    /// </summary>
    [HttpDelete]
    public void Delete(Guid? parent, string part, int index)
        => Real.Delete(parent, part, index);

    [HttpGet]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public List<ListSlotContentsInfo> Items(Guid parent, string part)
        => Real.Items(parent, part);


    // TODO: part should be handed in with all the relevant names! atm it's "content" in the content-block scenario
    [HttpPost]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public bool Items([FromUri] Guid parent, List<ListSlotContentsInfo> list, [FromUri] string part)
        => Real.Items(parent, list, part);

    [HttpGet]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public List<ListSlotContentsInfo> ContentBlockHeader(Guid parent)
        => Real.ContentBlockHeader(parent);

}