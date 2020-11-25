using System;
using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Formats;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.Dnn.WebApi.Context;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.WebApi.Cms;

namespace ToSic.Sxc.Dnn.WebApi.Cms
{
    [SupportedModules("2sxc,2sxc-app")]
    [ValidateAntiForgeryToken]
    public class EditController : SxcApiControllerBase, IEditController
    {
        protected override string HistoryLogName => "Api.UiCont";

        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public AllInOneDto Load([FromBody] List<ItemIdentifier> items, int appId)
            => _build<EditLoadBackend>().Init(Log)
                .Load(GetBlock(), new DnnContextBuilder(_serviceProvider, PortalSettings, ActiveModule), appId, items);

        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public Dictionary<Guid, int> Save([FromBody] AllInOneDto package, int appId, bool partOfPage) 
            => _build<EditSaveBackend>().Init(GetBlock(), Log)
                .Save(package, appId, partOfPage);

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
        public IEnumerable<EntityForPickerDto> EntityPicker([FromUri] int appId, [FromBody] string[] items,
            [FromUri] string contentTypeName = null, [FromUri] int? dimensionId = null)
            => _build<EntityPickerBackend>().Init(Log)
                .GetAvailableEntities(GetContext(), appId, items, contentTypeName, dimensionId);

    }
}
