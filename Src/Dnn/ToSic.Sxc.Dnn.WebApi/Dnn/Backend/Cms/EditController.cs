using ToSic.Eav.WebApi.Sys.Cms;
using ToSic.Eav.WebApi.Sys.Dto;
using ToSic.Sxc.Dnn.WebApi.Sys;
using RealController = ToSic.Sxc.Backend.Cms.EditControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Cms;

//[SupportedModules(DnnSupportedModuleNames)]
[ValidateAntiForgeryToken]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class EditController() : DnnSxcControllerBase(RealController.LogSuffix), IEditController
{
    #region Setup / Infrastructure

    private RealController Real => SysHlp.GetService<RealController>();

    #endregion

    /// <inheritdoc />
    [HttpPost]
    [AllowAnonymous] // will check security internally, so assume no requirements
    public EditLoadDto Load([FromBody] List<ItemIdentifier> items, int appId)
    {
        var l = Log.Fn<EditLoadDto>($"Items: {items.Count}, AppId: {appId}");
        return l.ReturnAsOk(Real.Load(items, appId));
    }


    /// <inheritdoc />
    [HttpPost]
    [AllowAnonymous] // will check security internally, so assume no requirements
    public Dictionary<Guid, int> Save([FromBody] EditSaveDto package, int appId, bool partOfPage)
    {
        var l = Log.Fn<Dictionary<Guid, int>>($"Items: {package?.Items?.Count}, AppId: {appId}, PartOfPage: {partOfPage}");
        return l.ReturnAsOk(Real.Save(package, appId, partOfPage));
    }

    /// <inheritdoc />
    [HttpGet]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    public LinkInfoDto LinkInfo(string link, int appId, string contentType = default, Guid guid = default,
        string field = default)
        => Real.LinkInfo(link, appId, contentType, guid, field);

    /// <inheritdoc />
    [HttpPost]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    public bool Publish(int id)
        => Real.Publish(id);
}