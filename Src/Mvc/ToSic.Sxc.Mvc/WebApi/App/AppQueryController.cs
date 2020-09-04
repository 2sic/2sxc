using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ToSic.Eav.Apps.Run;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Mvc.Run;
using ToSic.Sxc.WebApi.App;

// TODO: #MissingFeature
// 1. Query from context / header
// 2. GetContext using current context
// 3. Verify that additional query params would affect query results

namespace ToSic.Sxc.Mvc.WebApi.App
{
    [Route(WebApiConstants.Root + "/app/{appPath}/query/")]
    [ApiController]
    public class AppQueryController : SxcStatelessControllerBase
    {
        #region DI / Constructor
        protected override string HistoryLogName => "App.AppQry";

        #endregion

        private IInstanceContext GetContext()
        {
            // in case the initial request didn't yet find a block builder, we need to create it now
            var context = // BlockBuilder?.Context ??
                new InstanceContext(new MvcTenant(), new PageNull(), new ContainerNull(), new MvcUser());
            return context;
        }

        private IBlockBuilder BlockBuilder => null;

        [HttpGet("{name}")]
        public Dictionary<string, IEnumerable<Dictionary<string, object>>> PublicQuery(
            string appPath,
            string name,
            [FromQuery] string stream = null
        ) => new AppQuery().Init(Log).PublicQuery(GetContext(), appPath, name, stream, NoBlock);

    }
}
