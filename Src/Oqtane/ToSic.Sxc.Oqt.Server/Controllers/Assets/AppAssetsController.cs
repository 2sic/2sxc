using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Oqtane.Infrastructure;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing.Patterns;
using Oqtane.Repository;

namespace ToSic.Sxc.Oqt.Server.Controllers.Assets
{
    [Route("{alias}/api/sxc/assets")]
    public class AppAssetsController: ToSic.Sxc.Oqt.Server.Controllers.AppAssetsController
    {
        public override string Route => "assets";

        public AppAssetsController(ITenantResolver tenantResolver, IWebHostEnvironment hostingEnvironment, ILogManager logger) : base(tenantResolver, hostingEnvironment, logger)
        { }
    }
}
