using ToSic.Eav.Ui.Edit.Sys;
using ToSic.Eav.Ui.Edit.Sys.Load;
using ToSic.Eav.WebApi.Sys;
using ToSic.Eav.WebApi.Sys.Cms;
using ToSic.Eav.WebApi.Sys.Dto;
using ToSic.Sxc.Dnn.WebApi.Sys;
using ToSic.Sxc.Dnn.WebApi.Sys.HttpJson;
using RealController = ToSic.Sxc.Backend.Cms.EditControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Cms;

//[SupportedModules(DnnSupportedModuleNames)]
[DnnLogWebApi, DnnLogExceptions, ConfigureJsonOnlyResponse] // #CompositionOverInheritance
[ValidateAntiForgeryToken]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class EditController : DnnApiController, IEditController
{
    #region WebApi Composition Infrastructure

    public ILog Log => field ??= LogFactory.Create($"Api.{RealController.LogSuffix}");

    private DnnWebApiHelper SysHlp => field ??= new(this, EavWebApiConstants.HistoryNameWebApi);

    #endregion



    private RealController Real => SysHlp.GetService<RealController>();


    /// <inheritdoc />
    [HttpPost]
    [AllowAnonymous] // will check security internally, so assume no requirements
    public async Task<EditLoadDto> Load([FromBody] List<ItemIdentifier> items, int appId)
    {
        SysHlp.PublishContext(Request, new(TryBlock: true, TryApp: true, AppId: appId), Log);
        return await Real.Load(items, appId);
    }


    /// <inheritdoc />
    [HttpPost]
    [AllowAnonymous] // will check security internally, so assume no requirements
    public async Task<Dictionary<Guid, int>> Save([FromBody] EditSaveDto package, int appId, bool partOfPage)
    {
        SysHlp.PublishContext(Request, new(TryBlock: true, TryApp: true, AppId: appId), Log);
        return await Real.Save(package, appId, partOfPage);
    }

    /// <inheritdoc />
    [HttpGet]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    public LinkInfoDto LinkInfo(string link, int appId, string contentType = default, Guid guid = default, string field = default)
    {
        SysHlp.PublishContext(Request, new(TryBlock: true, TryApp: true, AppId: appId), Log);
        return Real.LinkInfo(link, appId, contentType, guid, field);
    }

    /// <inheritdoc />
    [HttpPost]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    public Task<bool> Publish(int id)
    {
        SysHlp.PublishContextBlockOnly(Request, Log);
        return Real.Publish(id);
    }
}