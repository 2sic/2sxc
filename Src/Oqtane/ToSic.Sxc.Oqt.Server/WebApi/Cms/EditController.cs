using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Oqtane.Shared;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Formats;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Eav.WebApi.Routing;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.WebApi.Cms;

namespace ToSic.Sxc.Oqt.Server.WebApi.Cms
{
    // Release routes
    [Route(WebApiConstants.ApiRootWithNoLang + $"/{AreaRoutes.Cms}")]
    [Route(WebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Cms}")]
    [Route(WebApiConstants.ApiRootPathNdLang + $"/{AreaRoutes.Cms}")]

    // Beta routes - TODO: @STV - why is this beta?
    [Route(WebApiConstants.WebApiStateRoot + $"/{AreaRoutes.Cms}")]

    [ValidateAntiForgeryToken]

    [ApiController]
    public class EditController: OqtStatefulControllerBase, IEditController
    {
        #region DI
        protected override string HistoryLogName => "Api.UiCntr";

        public EditController(EditControllerReal realController) => RealController = realController.Init(Log);
        private EditControllerReal RealController { get; }

        #endregion

        [HttpPost]
        // [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        [AllowAnonymous]   // will check security internally, so assume no requirements
        public EditDto Load([FromBody] List<ItemIdentifier> items, int appId)
            => RealController.Load(items, appId);

        [HttpPost]
        // [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        [Authorize(Roles = RoleNames.Admin)]
        public Dictionary<Guid, int> Save([FromBody] EditDto package, int appId, bool partOfPage)
            => RealController.Save(package, appId, partOfPage);

        [HttpGet]
        [HttpPost]
        [AllowAnonymous] // security check happens internally
        public async Task<IEnumerable<EntityForPickerDto>> EntityPicker(int appId, 
            /*[FromBody] string[] items,*/ string contentTypeName = null) // [FromBody] is commented because it is sometimes null and options.AllowEmptyInputInBodyModelBinding = true is not working
        {
            // get body as json (using this complicated way to read body because it is sometimes null and
            // options.AllowEmptyInputInBodyModelBinding = true or similar solutions are not working)
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();
            var items = JsonConvert.DeserializeObject<string[]>(body);

            return RealController.EntityPicker(appId, items, contentTypeName);
        }

        /// <inheritdoc />
        [HttpGet]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        //[Authorize(Roles = RoleNames.Everyone)] commented because of http403 issue
        // TODO: 2DM please check permissions
        public LinkInfoDto LinkInfo(string link, int appId, string contentType = default, Guid guid = default, string field = default)
            => RealController.LinkInfo(link, appId, contentType, guid, field);
    }
}
