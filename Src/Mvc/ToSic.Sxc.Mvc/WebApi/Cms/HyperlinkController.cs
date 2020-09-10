using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToSic.Sxc.WebApi.Cms;

// #Todo - change the official route of this - probably /cms/link/xyz

namespace ToSic.Sxc.Mvc.WebApi.Cms
{
    [ApiController]
    [Route(WebApiConstants.WebApiRoot + "/dnn/[controller]/[action]")]
    public class HyperlinkController: SxcStatefullControllerBase
    {
        protected override string HistoryLogName => WebApiConstants.MvcApiLogPrefix + "HypLnk";

        [HttpGet]
        [AllowAnonymous]   // will check security internally, so assume no requirements
        public string ResolveHyperlink(string hyperlink, int appId, string contentType = default, Guid guid = default, string field = default)
            => new HyperlinkBackend<int, int>().Init(Log).ResolveHyperlink(GetBlock(), hyperlink, appId, contentType, guid, field);

    }
}
