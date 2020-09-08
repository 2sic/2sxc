using System;
using Microsoft.AspNetCore.Mvc;
using ToSic.Sxc.WebApi.Cms;

// #Todo - change the official route of this - probably /cms/link/xyz

namespace ToSic.Sxc.Mvc.WebApi.Cms
{
    [ApiController]
    [Route(WebApiConstants.WebApiRoot + "/dnn/hyperlink/")]
    public class HyperlinkController: SxcStatefullControllerBase
    {
        protected override string HistoryLogName => WebApiConstants.MvcApiLogPrefix + "HypLnk";

        /// <summary>
        /// This overload is only for resolving page-references, which need fewer parameters
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public string ResolveHyperlink(string hyperlink, int appId)
            => ResolveHyperlink(hyperlink, appId, null, default, null);


        [HttpGet]
        //[AllowAnonymous]   // will check security internally, so assume no requirements
        public string ResolveHyperlink(string hyperlink, int appId, string contentType, Guid guid, string field)
            => new HyperlinkBackend().Init(Log).ResolveHyperlink(GetBlock(), hyperlink, appId, contentType, guid, field);

    }
}
