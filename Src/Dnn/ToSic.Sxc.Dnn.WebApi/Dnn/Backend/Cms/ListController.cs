using ToSic.Eav.WebApi.Sys.Cms;
using ToSic.Sxc.Dnn.WebApi.Sys;
using RealController = ToSic.Sxc.Backend.Cms.ListControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Cms;

[SupportedModules(DnnSupportedModuleNames)]
[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class ListController() : DnnSxcControllerBase(RealController.LogSuffix), IListController
{
    private RealController Real => SysHlp.GetService<RealController>();

    [HttpPost]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public void Replace(Guid parent, string part, int index, int entityId, bool add = false)
        => Real.Replace(parent, part, index, entityId, add);


    [HttpGet]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public ReplacementListDto ReplaceOptions(Guid parent, string part, int index)
        => Real.ReplaceOptions(parent, part, index);

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
    public void Delete(Guid? parent, string fields, int index)
        => Real.Delete(parent, fields, index);

}