using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Infrastructure;
using Oqtane.Repository;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Controllers.Assets
{
    // Release routes
    [Route(WebApiConstants.ApiRoot + "/assets")]
    [Route(WebApiConstants.ApiRoot2 + "/assets")]
    [Route(WebApiConstants.ApiRoot3 + "/assets")]

    // Beta routes
    [Route(WebApiConstants.WebApiStateRoot +"/assets")]
    public class AppAssetsController: ToSic.Sxc.Oqt.Server.Controllers.AppAssetsController
    {
        public override string Route => "assets";

        public AppAssetsController(ITenantResolver tenantResolver, IWebHostEnvironment hostingEnvironment, ILogManager logger) : base(tenantResolver, hostingEnvironment, logger)
        { }
    }
}
