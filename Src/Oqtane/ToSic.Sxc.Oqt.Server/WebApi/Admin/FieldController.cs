using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Sxc.Oqt.Server.Controllers;
using RealController = ToSic.Eav.WebApi.Sys.Admin.FieldControllerReal;
// ReSharper disable RouteTemplates.MethodMissingRouteParameters

namespace ToSic.Sxc.Oqt.Server.WebApi.Admin;

[ValidateAntiForgeryToken]
[Authorize(Roles = RoleNames.Admin)]

// Release routes
[Route(OqtWebApiConstants.ApiRootNoLanguage + $"/{AreaRoutes.Admin}")]
[Route(OqtWebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Admin}")]
[Route(OqtWebApiConstants.ApiRootPathAndLang + $"/{AreaRoutes.Admin}")]

[ShowApiWhenReleased(ShowApiMode.Never)]
public class FieldController() : OqtStatefulControllerBase(RealController.LogSuffix), IFieldController
{
    private RealController Real => GetService<RealController>();

    #region Fields - Get, Reorder, Data-Types (for dropdown), etc.

    /// <summary>
    /// Used to be GET ContentType/AddField
    /// </summary>
    [HttpPost]
    public int Add(int appId, int contentTypeId, string staticName, string type, string inputType, int index)
        => Real.Add(appId, contentTypeId, staticName, type, inputType, index);

    /// <summary>
    /// Used to be GET ContentType/DeleteField
    /// </summary>
    [HttpDelete]
    public bool Delete(int appId, int contentTypeId, int attributeId)
        => Real.Delete(appId, contentTypeId, attributeId);

    /// <summary>
    /// Used to be GET ContentType/Reorder
    /// </summary>
    [HttpPost]
    public bool Sort(int appId, int contentTypeId, string order)
        => Real.Sort(appId, contentTypeId, order);


    /// <summary>
    /// Used to be GET ContentType/UpdateInputType
    /// </summary>
    [HttpPost]
    public bool InputType(int appId, int attributeId, string inputType)
        => Real.InputType(appId, attributeId, inputType);

    #endregion

    /// <summary>
    /// Used to be GET ContentType/Rename
    /// </summary>
    [HttpPost]
    public void Rename(int appId, int contentTypeId, int attributeId, string newName)
        => Real.Rename(appId, contentTypeId, attributeId, newName);

    #region Sharing and Inheriting

    // 2rb: GetSharedFields, GetAncestors and GetDescendants were replaced by the
    // System.SharedFields DataSource through query System.SysData.

    [HttpPost]
    public bool Share(int appId, int attributeId, bool share, bool hide = false)
        => Real.Share(appId, attributeId, share, hide);

    [HttpPost]
    public bool Inherit(int appId, int attributeId, Guid inheritMetadataOf)
        => Real.Inherit(appId, attributeId, inheritMetadataOf);

    [HttpPost]
    public bool AddInheritedField(int appId, int contentTypeId, string sourceType, Guid sourceField, string name)
        => Real.AddInheritedField(appId, contentTypeId, sourceType, sourceField, name);


    #endregion

}