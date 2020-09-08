using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToSic.Eav.WebApi.Formats;
using ToSic.Sxc.Mvc.WebApi.Context;
using ToSic.Sxc.WebApi.Cms;

namespace ToSic.Sxc.Mvc.WebApi.Cms
{
    [Route(WebApiConstants.WebApiRoot + "/eav/ui/")]
    [ApiController]
    public class UiController: SxcStatefullControllerBase
    {
        protected override string HistoryLogName => WebApiConstants.MvcApiLogPrefix + "UiCntr";

        [HttpGet]
        [AllowAnonymous]   // will check security internally, so assume no requirements
        public AllInOneDto Load([FromBody] List<ItemIdentifier> items, int appId)
        {
            return new EditLoadBackend(null)
                .Init(Log)
                .Load(GetBlock(), new MvcContextBuilder(), appId, items);
        }
    }
}
