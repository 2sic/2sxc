using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Eav.DataSources.Query.Dev.Sys;
using ToSic.Eav.ImportExport.Sys;
using ToSic.Eav.WebApi.Admin.Sys;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.WebApi.Sys.ActionFilters;
using RealController = ToSic.Sxc.Backend.Admin.Query.QueryControllerReal;

namespace ToSic.Sxc.Oqt.Server.WebApi.Admin;

/// <summary>
/// Proxy Class to the EAV PipelineDesignerController (Web API Controller)
/// </summary>
[ValidateAntiForgeryToken]
[Authorize(Roles = RoleNames.Admin)]

// Release routes
[Route(OqtWebApiConstants.ApiRootNoLanguage + $"/{AreaRoutes.Admin}")]
[Route(OqtWebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Admin}")]
[Route(OqtWebApiConstants.ApiRootPathAndLang + $"/{AreaRoutes.Admin}")]

[OqtApiRequest]
[SystemTextJsonFormatter]
[ServiceFilter(typeof(OptionalBodyFilter))]
[ServiceFilter(typeof(HttpResponseExceptionFilter))]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class QueryController : ControllerBase, IHasLog, IQueryController
{
    /// <inheritdoc />
    public ILog Log => field ??= LogFactory.Create($"Api.{RealController.LogSuffix}", null, GetType().Name);

    private RealController Real => this.GetApiService<RealController>();

    [HttpPost]
    public QueryDefinitionDto Save([FromBody] QueryDefinitionDto data, int appId, int id) =>
        Real.Save(data, appId, id);

    [HttpGet]
    public QueryRunDto RunDev(int appId, int id, int top = 0)
    {
        this.PrepareRequestBlock();
        return Real.RunDev(appId, id, top);
    }

    [HttpGet]
    public QueryRunDto DebugStream(int appId, int id, string from, string @out, int top = 25)
    {
        this.PrepareRequestBlock();
        return Real.DebugStream(appId, id, @from, @out, top);
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
