using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToSic.Eav.Run;
using ToSic.Sxc.WebApi.Cms;

// #Todo - change the official route of this - probably /cms/link/xyz

namespace ToSic.Sxc.OqtaneModule.Server.Controllers
{
    [ApiController]
    [Route(WebApiConstants.WebApiRoot + "/dnn/[controller]/[action]")]
    public class HyperlinkController: SxcStatefulControllerBase
    {
        protected override string HistoryLogName => WebApiConstants.MvcApiLogPrefix + "HypLnk";

        public HyperlinkController(SxcOqtane sxcOqtane, IZoneMapper zoneMapper) : base(sxcOqtane, zoneMapper) { }


        [HttpGet]
        [AllowAnonymous]   // will check security internally, so assume no requirements
        public string ResolveHyperlink(string hyperlink, int appId, string contentType = default, Guid guid = default, string field = default)
            => new HyperlinkBackend<int, int>().Init(Log).ResolveHyperlink(GetBlock(), hyperlink, appId, contentType, guid, field);

    }
}
