using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Formats;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.WebApi.Cms;

namespace IntegrationSamples.SxcEdit01.Controllers
{
    //[AutoValidateAntiforgeryToken]
    [Route(WebApiConstants.DefaultRouteRoot + "/cms" + WebApiConstants.DefaultRouteControllerAction)]
    [ApiController]
    public class EditController: IntStatefulControllerBase, IEditController
    {
        #region DI
        protected override string HistoryLogName => IntegrationConstants.LogPrefix + ".UiCntr";

        public EditController(EditControllerReal realController) => RealController = realController.Init(Log);
        private EditControllerReal RealController { get; }

        #endregion


        [HttpPost]
        [AllowAnonymous]   // will check security internally, so assume no requirements
        public EditDto Load([FromBody] List<ItemIdentifier> items, int appId)
            => RealController.Load(items, appId);

        [HttpPost]
        // todo #mvcSec [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public Dictionary<Guid, int> Save([FromBody] EditDto package, int appId, bool partOfPage)
            => RealController.Save(package, appId, partOfPage);

        [HttpGet]
        [HttpPost]
        [AllowAnonymous] // security check happens internally
        public IEnumerable<EntityForPickerDto> EntityPicker(int appId, [FromBody] string[] items, string contentTypeName = null)
            => RealController.EntityPicker(appId, items, contentTypeName);

        /// <inheritdoc />
        [HttpGet]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public LinkInfoDto LinkInfo(string link, int appId, string contentType = default, Guid guid = default, string field = default)
            => RealController.LinkInfo(link, appId, contentType, guid, field);

    }
}
