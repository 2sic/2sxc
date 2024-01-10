using System;
using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.WebApi.Cms;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Formats;
using ToSic.Lib.Logging;
using ToSic.Sxc.WebApi;
using RealController = ToSic.Sxc.Backend.Cms.EditControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Cms;

//[SupportedModules(DnnSupportedModuleNames)]
[ValidateAntiForgeryToken]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class EditController() : SxcApiControllerBase(RealController.LogSuffix), IEditController
{
    #region Setup / Infrastructure

    private RealController Real => SysHlp.GetService<RealController>();

    #endregion

    /// <inheritdoc />
    [HttpPost]
    [AllowAnonymous] // will check security internally, so assume no requirements
    public EditDto Load([FromBody] List<ItemIdentifier> items, int appId)
    {
        var l = Log.Fn<EditDto>($"Items: {items.Count}, AppId: {appId}");
        return l.ReturnAsOk(Real.Load(items, appId));
    }


    /// <inheritdoc />
    [HttpPost]
    [AllowAnonymous] // will check security internally, so assume no requirements
    public Dictionary<Guid, int> Save([FromBody] EditDto package, int appId, bool partOfPage)
    {
        var l = Log.Fn<Dictionary<Guid, int>>($"Items: {package?.Items?.Count}, AppId: {appId}, PartOfPage: {partOfPage}");
        return l.ReturnAsOk(Real.Save(package, appId, partOfPage));
    }


    /// <inheritdoc />
    [HttpGet]
    [HttpPost]
    [AllowAnonymous] // security check happens internally
    public IEnumerable<EntityForPickerDto> EntityPicker(
        [FromUri] int appId,
        [FromBody] string[] items,
        [FromUri] string contentTypeName = null)
        => Real.EntityPicker(appId, items, contentTypeName);

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