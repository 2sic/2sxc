using System.Collections.Generic;
using System.Web;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.Apps;
using ToSic.Eav.DataSource.Query;
using ToSic.Eav.Plumbing;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.Dnn.WebApi.Logging;
using ToSic.Sxc.WebApi;
using RealController = ToSic.Sxc.WebApi.Admin.Query.QueryControllerReal;

namespace ToSic.Sxc.Dnn.WebApi.Admin
{
	/// <summary>
	/// Proxy Class to the EAV PipelineDesignerController (Web API Controller)
	/// </summary>
	[SupportedModules(DnnSupportedModuleNames)]
	[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [DnnLogExceptions]
    [ValidateAntiForgeryToken]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
	public class QueryController : SxcApiControllerBase, IQueryController
    {
        public QueryController() : base(RealController.LogSuffix, RealController.LogGroup,
            firstMessage: $"Query: {HttpContext.Current?.Request.Url.AbsoluteUri.After("/query/")}") { }

        private RealController Real => SysHlp.GetService<RealController>();

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
}