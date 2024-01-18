using ToSic.Eav.WebApi.Cms;
using RealController = ToSic.Sxc.Backend.Cms.ListControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Cms;

[SupportedModules(DnnSupportedModuleNames)]
[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class ListController() : DnnSxcControllerBase(RealController.LogSuffix), IListController
{
    private RealController Real => SysHlp.GetService<RealController>();

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