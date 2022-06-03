using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using System.Collections.Generic;
using ToSic.Eav.WebApi.Routing;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.WebApi.Admin;

namespace ToSic.Sxc.Oqt.Server.WebApi.Admin
{
    // Release routes
    [Route(WebApiConstants.ApiRootWithNoLang + $"/{AreaRoutes.Admin}")]
    [Route(WebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Admin}")]
    [Route(WebApiConstants.ApiRootPathNdLang + $"/{AreaRoutes.Admin}")]

    [ValidateAntiForgeryToken]
    //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    [Authorize(Roles = RoleNames.Admin)]

    public class CodeController : OqtControllerBase<CodeControllerReal>
    {
        public CodeController(string logSuffix) : base(CodeControllerReal.LogSuffix) { }

        [HttpGet]
        public IEnumerable<CodeControllerReal.HelpItem> InlineHelp(string language) => Real.InlineHelp(language);
    }
}
