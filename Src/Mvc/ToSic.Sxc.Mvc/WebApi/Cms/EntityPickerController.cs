using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToSic.Eav.WebApi.Dto;
using ToSic.Sxc.WebApi.Cms;

namespace ToSic.Sxc.Mvc.WebApi.Cms
{
    [Route(WebApiConstants.WebApiRoot + "/eav/[controller]/[action]")]
    [ApiController]
    public class EntityPickerController: SxcStatefullControllerBase
    {
        #region DI
        protected override string HistoryLogName => WebApiConstants.MvcApiLogPrefix + "EntPck";

        #endregion

        [HttpGet]
        [HttpPost]
        [AllowAnonymous]
        public IEnumerable<EntityForPickerDto> GetAvailableEntities(int appId, [FromBody] string[] items = null, string contentTypeName = null, int? dimensionId = null) 
            => new EntityPickerBackend().Init(Log).GetAvailableEntities(GetContext(), appId, items, contentTypeName, dimensionId);
    }
}
