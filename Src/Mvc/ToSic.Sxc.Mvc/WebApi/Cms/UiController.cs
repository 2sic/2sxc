using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToSic.Eav.WebApi.Formats;
using ToSic.Sxc.Mvc.WebApi.Context;
using ToSic.Sxc.WebApi.Cms;

namespace ToSic.Sxc.Mvc.WebApi.Cms
{
    [Route(WebApiConstants.WebApiRoot + "/eav/ui/[action]")]
    [ApiController]
    public class UiController: SxcStatefullControllerBase
    {
        #region DI
        protected override string HistoryLogName => WebApiConstants.MvcApiLogPrefix + "UiCntr";

        public UiController(MvcContextBuilder contextBuilder) => _contextBuilder = contextBuilder;
        private readonly MvcContextBuilder _contextBuilder;

        #endregion


        [HttpGet]
        [AllowAnonymous]   // will check security internally, so assume no requirements
        public string Ping() => "test ping";

        [HttpPost]
        [AllowAnonymous]   // will check security internally, so assume no requirements
        public AllInOneDto Load([FromBody] List<ItemIdentifier> items, int appId)
        {
            var block = GetBlock();
            var result = new EditLoadBackend()
                .Init(Log)
                .Load(block, _contextBuilder.Init(block), appId, items);
            return result;
        }

        [HttpPost]
        // todo #mvcSec [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public Dictionary<Guid, int> Save([FromBody] AllInOneDto package, int appId, bool partOfPage)
            => new EditSaveBackend().Init(Log)
                .Save(GetBlock(), package, appId, partOfPage);

    }
}
