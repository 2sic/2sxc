using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Repository;
using ToSic.Eav.Run;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Formats;
using ToSic.Sxc.WebApi.Cms;

namespace ToSic.Sxc.OqtaneModule.Server.Controllers
{
    [Route(WebApiConstants.WebApiRoot + "/cms/edit/[action]")]
    [ApiController]
    public class EditController: SxcStatefulControllerBase
    {
        #region DI
        protected override string HistoryLogName => WebApiConstants.MvcApiLogPrefix + "UiCntr";

        public EditController(OqtaneContextBuilder contextBuilder, SxcOqtane sxcOqtane, IZoneMapper zoneMapper, ITenantResolver tenantResolver, IHttpContextAccessor httpContextAccessor, IAliasRepository aliasRepository)
            : base(sxcOqtane, zoneMapper, tenantResolver, httpContextAccessor, aliasRepository)
        {
            _contextBuilder = contextBuilder;
        }

        private readonly OqtaneContextBuilder _contextBuilder;

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

        /// <summary>
        /// Used to be GET Ui/GetAvailableEntities
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="items"></param>
        /// <param name="contentTypeName"></param>
        /// <param name="dimensionId"></param>
        /// <returns></returns>
        [HttpGet]
        [HttpPost]
        [AllowAnonymous] // security check happens internally
        public IEnumerable<EntityForPickerDto> EntityPicker(int appId, [FromBody] string[] items,
            string contentTypeName = null, int? dimensionId = null)
            => new EntityPickerBackend().Init(Log)
                .GetAvailableEntities(GetContext(), appId, items, contentTypeName, dimensionId);

    }
}
