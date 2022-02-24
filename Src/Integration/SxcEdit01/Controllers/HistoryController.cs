using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using ToSic.Eav.Persistence.Versions;
using ToSic.Eav.WebApi.Cms;
using ToSic.Eav.WebApi.Formats;
using ToSic.Eav.WebApi.Routing;
using ToSic.Sxc.WebApi.Cms;

namespace IntegrationSamples.SxcEdit01.Controllers
{
    // Release routes
    [Route(IntegrationConstants.DefaultRouteRoot + AreaRoutes.Cms)]

    //[ValidateAntiForgeryToken]
    public class HistoryController : IntControllerBase<HistoryControllerReal>, IHistoryController
    {
        public HistoryController(): base(HistoryControllerReal.LogSuffix) { }

        /// <inheritdoc />
        [HttpPost]
        public List<ItemHistory> Get(int appId, [FromBody] ItemIdentifier item) 
            => Real.Get(appId, item);

        /// <inheritdoc />
        [HttpPost]
        public bool Restore(int appId, int changeId, [FromBody] ItemIdentifier item) =>
            Real.Restore(appId, changeId, item);
    }
}