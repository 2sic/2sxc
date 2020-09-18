using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Web.Api;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.PublicApi;

namespace ToSic.Sxc.WebApi.Cms
{
    [SupportedModules("2sxc,2sxc-app")]
    [ValidateAntiForgeryToken]
    public class EntityPickerController : SxcApiControllerBase, IEntityPickerController
    {
        [HttpGet]
        [HttpPost]
        [AllowAnonymous] // security check happens internally
        public IEnumerable<EntityForPickerDto> GetAvailableEntities([FromUri] int appId, [FromBody] string[] items,
            [FromUri] string contentTypeName = null, [FromUri] int? dimensionId = null)
            => new EntityPickerBackend().Init(Log)
                .GetAvailableEntities(GetContext(), appId, items, contentTypeName, dimensionId);
    }
}