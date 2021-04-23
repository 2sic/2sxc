using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Infrastructure;
using Oqtane.Repository;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Controllers.Adam
{
    // Release routes
    [Route(WebApiConstants.ApiRoot + "/adam")]
    [Route(WebApiConstants.ApiRoot2 + "/adam")]
    [Route(WebApiConstants.ApiRoot3 + "/adam")]

    // Beta routes
    [Route(WebApiConstants.WebApiStateRoot + "/adam")]
    public class AppAssetsController: ToSic.Sxc.Oqt.Server.Controllers.AppAssetsController
    {
        public override string Route => "adam";

        public AppAssetsController(ITenantResolver tenantResolver, IWebHostEnvironment hostingEnvironment,
            ILogManager logger) : base(tenantResolver, hostingEnvironment, logger)
        {

        }
    }
}
