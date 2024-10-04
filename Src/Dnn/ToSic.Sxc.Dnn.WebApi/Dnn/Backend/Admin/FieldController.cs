using ToSic.Eav.Apps.Internal.Work;
using ToSic.Eav.Data;
using ToSic.Eav.WebApi.Admin;
using ToSic.Eav.WebApi.Dto;
using RealController = ToSic.Eav.WebApi.Admin.FieldControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Admin;

/// <summary>
/// Web API Controller for Content-Type structures, fields etc.
/// </summary>
[SupportedModules(DnnSupportedModuleNames)]
[ValidateAntiForgeryToken]
[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class FieldController() : DnnSxcControllerBase(RealController.LogSuffix), IFieldController
{
    private RealController Real => SysHlp.GetService<RealController>();

    #region Fields - Get, Reorder, Data-Types (for dropdown), etc.

    /// <summary>
    /// Returns the configuration for a content type
    /// </summary>
    [HttpGet]
    public IEnumerable<ContentTypeFieldDto> All(int appId, string staticName) => Real.All(appId, staticName);

    /// <summary>
    /// Used to be GET ContentType/DataTypes
    /// </summary>
    [HttpGet]
    public string[] DataTypes(int appId) => Real.DataTypes(appId);

    /// <summary>
    /// Used to be GET ContentType/InputTypes
    /// </summary>
    [HttpGet]
    public List<InputTypeInfo> InputTypes(int appId) => Real.InputTypes(appId);

    /// <inheritdoc />
    [HttpGet]
    public Dictionary<string, string> ReservedNames() => Attributes.ReservedNames;
        
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
    public bool Delete(int appId, int contentTypeId, int attributeId) => Real.Delete(appId, contentTypeId, attributeId);

    /// <summary>
    /// Used to be GET ContentType/Reorder
    /// </summary>
    [HttpPost]
    public bool Sort(int appId, int contentTypeId, string order) => Real.Sort(appId, contentTypeId, order);


    /// <summary>
    /// Used to be GET ContentType/UpdateInputType
    /// </summary>
    [HttpPost]
    public bool InputType(int appId, int attributeId, string inputType) => Real.InputType(appId, attributeId, inputType);


    #endregion

    /// <summary>
    /// Used to be GET ContentType/Rename
    /// </summary>
    [HttpPost]
    public void Rename(int appId, int contentTypeId, int attributeId, string newName)
        => Real.Rename(appId, contentTypeId, attributeId, newName);


    #region Sharing and Inheriting

    [HttpGet]
    public IEnumerable<ContentTypeFieldDto> GetSharedFields(int appId, int attributeId = default)
        => Real.GetSharedFields(appId, attributeId);

    [HttpGet]
    public IEnumerable<ContentTypeFieldDto> GetAncestors(int appId, int attributeId)
        => Real.GetAncestors(appId, attributeId);

    [HttpGet]
    public IEnumerable<ContentTypeFieldDto> GetDescendants(int appId, int attributeId)
        => Real.GetDescendants(appId, attributeId);
    
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