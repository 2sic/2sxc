using ToSic.Eav.DataSources.Query.Dev.Sys;
using ToSic.Eav.ImportExport.Sys;
using ToSic.Eav.WebApi.Admin.Sys;
using ToSic.Eav.WebApi.Sys;
using ToSic.Sxc.Dnn.WebApi.Sys;
using ToSic.Sxc.Dnn.WebApi.Sys.HttpJson;
using RealController = ToSic.Sxc.Backend.Admin.Query.QueryControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Admin;

/// <summary>
/// Proxy Class to the EAV PipelineDesignerController (Web API Controller)
/// </summary>
[SupportedModules(DnnSupportedModuleNames)]
[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
[DnnLogWebApi, DnnLogExceptions, ConfigureJsonOnlyResponse] // #CompositionOverInheritance
[ValidateAntiForgeryToken]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class QueryController : DnnApiController, IQueryController
{
    #region WebApi Composition Infrastructure

    public ILog Log => field ??= LogFactory.Create("Api." + RealController.LogSuffix);

    private DnnWebApiHelper SysHlp => field ??= new(this, EavWebApiConstants.HistoryNameWebApi + RealController.LogGroup);

    #endregion


    private RealController Real => SysHlp.GetService<RealController>();

    [HttpPost]
    public QueryDefinitionDto Save([FromBody] QueryDefinitionDto data, int appId, int id) =>
        Real.Save(data, appId, id);

    [HttpGet]
    public QueryRunDto RunDev(int appId, int id, int top = 0)
    {
        // The block is required, so that queries can resolve things such as Module.Id etc.
        new DnnSxcWebApiInitBlockContextAndShare().InitBlockSpecs(SysHlp, Request, Log);
        return Real.RunDev(appId, id, top);
    }

    [HttpGet]
    public QueryRunDto DebugStream(int appId, int id, string from, string @out, int top = 25)
    {
        // The block is required, so that queries can resolve things such as Module.Id etc.
        new DnnSxcWebApiInitBlockContextAndShare().InitBlockSpecs(SysHlp, Request, Log);
        return Real.DebugStream(appId, id, from, @out, top);
    }

    [HttpGet]
    public void Clone(int appId, int id) =>
        Real.Clone(appId, id);

    [HttpDelete]
    public bool Delete(int appId, int id) =>
        Real.DeleteIfUnused(appId, id);

    [HttpPost]
    public bool Import(DataFileImportOptions args) =>
        Real.Import(args);
}