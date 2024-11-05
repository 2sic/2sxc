using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Eav.Apps;
using ToSic.Eav.DataSource.Internal.Query;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Eav.WebApi.Routing;
using ToSic.Sxc.Oqt.Server.Controllers;
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

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class QueryController() : OqtStatefulControllerBase(RealController.LogSuffix), IQueryController
{
    private RealController Real => GetService<RealController>();

    [HttpGet] public QueryDefinitionDto Get(int appId, int? id = null) => Real.Get(appId, id);

    [HttpGet] public IEnumerable<DataSourceDto> DataSources(int zoneId, int appId) => Real.DataSources(new AppIdentity(zoneId, appId));

    [HttpPost] public QueryDefinitionDto Save([FromBody] QueryDefinitionDto data, int appId, int id)
        => Real.Save(data, appId, id);

    [HttpGet] public QueryRunDto Run(int appId, int id, int top = 0) => Real.RunDev(appId, id, top);

    [HttpGet] public QueryRunDto DebugStream(int appId, int id, string from, string @out, int top = 25) 
        => Real.DebugStream(appId, id, @from, @out, top);

    [HttpGet] public void Clone(int appId, int id) => Real.Clone(appId, id);

    [HttpDelete] public bool Delete(int appId, int id) => Real.DeleteIfUnused(appId, id);

    [HttpPost] public bool Import(EntityImportDto args) => Real.Import(args);
}