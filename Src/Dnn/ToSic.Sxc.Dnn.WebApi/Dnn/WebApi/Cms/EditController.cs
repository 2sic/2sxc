using System;
using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Formats;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.WebApi.Cms;

namespace ToSic.Sxc.Dnn.WebApi.Cms
{
    [SupportedModules("2sxc,2sxc-app")]
    [ValidateAntiForgeryToken]
    public class EditController : SxcApiControllerBase, IEditController
    {
        protected override string HistoryLogName => "Api.Edit";

        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public AllInOneDto Load([FromBody] List<ItemIdentifier> items, int appId)
            => _build<EditLoadBackend>().Init(Log)
                .Load(appId, items);

        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public Dictionary<Guid, int> Save([FromBody] AllInOneDto package, int appId, bool partOfPage) 
            => _build<EditSaveBackend>().Init(appId, Log)
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
                .GetAvailableEntities(appId, items, contentTypeName, dimensionId);

        /// <summary>
        /// This call will resolve links to files and to pages.
        /// For page-resolving, it only needs Hyperlink and AppId.
        /// For file resolves it needs the item context so it can verify that an item is in the ADAM folder of this object.
        /// </summary>
        /// <param name="link">The link to resolve - required. Can be a real link or a file:xx page:xx reference</param>
        /// <param name="appId">App id to which this link (or or file/page) belongs to</param>
        /// <param name="contentType">Content Type (optional). Relevant for checking ADAM links inside an item.</param>
        /// <param name="guid">Item GUID (optional). Relevant for checking ADAM links inside an item.</param>
        /// <param name="field">Item field (optional). Relevant for checking ADAM links inside an item.</param>
        /// <returns></returns>
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public string LookupLink(string link, int appId, string contentType = default, Guid guid = default, string field = default)
            => _build<HyperlinkBackend<int, int>>().Init(Log).ResolveHyperlink(appId, link, contentType, guid, field);

    }
}
