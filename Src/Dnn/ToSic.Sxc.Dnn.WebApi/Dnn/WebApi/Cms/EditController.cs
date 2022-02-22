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

        private EditControllerReal RealController => GetService<EditControllerReal>().Init(Log);

        /// <inheritdoc />
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public EditDto Load([FromBody] List<ItemIdentifier> items, int appId)
            => RealController.Load(items, appId);

        /// <inheritdoc />
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public Dictionary<Guid, int> Save([FromBody] EditDto package, int appId, bool partOfPage)
            => RealController.Save(package, appId, partOfPage);

        /// <inheritdoc />
        [HttpGet]
        [HttpPost]
        [AllowAnonymous] // security check happens internally
        public IEnumerable<EntityForPickerDto> EntityPicker(
            [FromUri] int appId,
            [FromBody] string[] items,
            [FromUri] string contentTypeName = null)
            => RealController.EntityPicker(appId, items, contentTypeName);

        /// <inheritdoc />
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public LinkInfoDto LinkInfo(string link, int appId, string contentType = default, Guid guid = default,
            string field = default)
            => RealController.LinkInfo(link, appId, contentType, guid, field);
    }
}
