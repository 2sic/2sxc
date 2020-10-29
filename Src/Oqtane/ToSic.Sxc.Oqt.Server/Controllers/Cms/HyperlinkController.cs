using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.WebApi.Cms;

// #Todo - change the official route of this - probably /cms/link/xyz

namespace ToSic.Sxc.Oqt.Server.Controllers
{
    [ApiController]
    [Route(WebApiConstants.WebApiStateRoot + "/dnn/[controller]/[action]")]
    [Route(WebApiConstants.WebApiStateRoot + "/cms/link/[action]")]
    public class HyperlinkController: SxcStatefulControllerBase
    {
        protected override string HistoryLogName => WebApiConstants.MvcApiLogPrefix + "HypLnk";

        public HyperlinkController(StatefulControllerDependencies dependencies) : base(dependencies) { }

        // new: will replace ResolveHyperlink
        [HttpGet]
        [AllowAnonymous]   // will check security internally, so assume no requirements
        public IActionResult Resolve(string hyperlink, int appId, string contentType = default, Guid guid = default, string field = default)
        {
            var result = new HyperlinkBackend<int, int>().Init(Log)
                .ResolveHyperlink(GetBlock(), hyperlink, appId, contentType, guid, field);
            return Json(result);
        }


        [HttpGet]
        [AllowAnonymous]   // will check security internally, so assume no requirements
        public IActionResult ResolveHyperlink(string hyperlink, int appId, string contentType = default, Guid guid = default, string field = default)
        {
            var result = new HyperlinkBackend<int, int>().Init(Log)
                .ResolveHyperlink(GetBlock(), hyperlink, appId, contentType, guid, field);
            return Json(result);
        }
    }
}
