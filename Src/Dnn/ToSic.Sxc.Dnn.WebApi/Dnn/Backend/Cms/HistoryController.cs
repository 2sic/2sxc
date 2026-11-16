using ToSic.Eav.WebApi.Sys;
using ToSic.Eav.WebApi.Sys.Cms;
using ToSic.Eav.WebApi.Sys.Dto;
using ToSic.Sxc.Dnn.WebApi.Sys;
using ToSic.Sxc.Dnn.WebApi.Sys.HttpJson;
using RealController = ToSic.Sxc.Backend.Cms.HistoryControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Cms;

/// <summary>
/// Controller for history of entities
/// </summary>
[PrivateApi]
[DnnLogWebApi, DnnLogExceptions, ConfigureJsonOnlyResponse] // #CompositionOverInheritance
[SupportedModules(DnnSupportedModuleNames)]
[ValidateAntiForgeryToken]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class HistoryController : DnnApiController, IHistoryController
{
    #region WebApi Composition Infrastructure

    public ILog Log => field ??= LogFactory.Create("Api." + RealController.LogSuffix);

    private DnnWebApiHelper SysHlp => field ??= new(this, EavWebApiConstants.HistoryNameWebApi);

    #endregion

    private RealController Real => SysHlp.GetService<RealController>();

    /// <inheritdoc />
    [HttpPost]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public bool Restore(int appId, int changeId, [FromBody] ItemIdentifier item) 
        => Real.Restore(appId, changeId, item);
}