using ToSic.Eav.Persistence.Versions;
using ToSic.Eav.WebApi.Cms;
using ToSic.Eav.WebApi.Formats;
using RealController = ToSic.Sxc.Backend.Cms.HistoryControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Cms;

/// <summary>
/// Controller for history of entities
/// </summary>
[PrivateApi]
[SupportedModules(DnnSupportedModuleNames)]
[ValidateAntiForgeryToken]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class HistoryController() : DnnSxcControllerBase(RealController.LogSuffix), IHistoryController
{
    private RealController Real => SysHlp.GetService<RealController>();

    /// <inheritdoc />
    [HttpPost]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public List<ItemHistory> Get(int appId, [FromBody] ItemIdentifier item)
        => Real.Get(appId, item);

    /// <inheritdoc />
    [HttpPost]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public bool Restore(int appId, int changeId, [FromBody] ItemIdentifier item) 
        => Real.Restore(appId, changeId, item);
}