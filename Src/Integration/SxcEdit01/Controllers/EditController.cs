using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ToSic.Eav.WebApi.Sys.Cms;
using ToSic.Eav.WebApi.Sys.Dto;
using ToSic.Eav.WebApi.Sys.Routing;
using ToSic.Sxc.Backend.Cms;

namespace IntegrationSamples.SxcEdit01.Controllers
{
    //[AutoValidateAntiforgeryToken]
    [Route(IntegrationConstants.DefaultRouteRoot + AreaRoutes.Cms)]
    [ApiController]
    public class EditController: IntControllerBase<EditControllerReal>, IEditController
    {
        // IMPORTANT: Uses the Proxy/Real concept - see https://r.2sxc.org/proxy-controllers

        public EditController() : base(EditControllerReal.LogSuffix) { }


        [HttpPost]
        [AllowAnonymous] // Anonymous is ok, security check happens internally
        public async Task<EditLoadDto> Load([FromBody] List<ItemIdentifier> items, int appId)
            => await Real.Load(items, appId);

        [HttpPost]
        // todo #mvcSec [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public async Task<Dictionary<Guid, int>> Save([FromBody] EditSaveDto package, int appId, bool partOfPage)
            => await Real.Save(package, appId, partOfPage);

        /// <inheritdoc />
        [HttpGet]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public LinkInfoDto LinkInfo(string link, int appId, string contentType = default, Guid guid = default, string field = default)
            => Real.LinkInfo(link, appId, contentType, guid, field);

        /// <inheritdoc />
        [HttpPost]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public bool Publish(int id)
            => Real.Publish(id);
    }
}
