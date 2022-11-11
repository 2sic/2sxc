using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System;
using System.Collections.Generic;
using System.Web.Http;
using ToSic.Eav.Logging;
using ToSic.Eav.WebApi.Cms;
using ToSic.Eav.WebApi.Dto;
using ToSic.Eav.WebApi.Formats;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.WebApi.Cms;

namespace ToSic.Sxc.Dnn.WebApi.Cms
{
    //[SupportedModules(DnnSupportedModuleNames)]
    [ValidateAntiForgeryToken]
    public class EditController : SxcApiControllerBase<EditControllerReal>, IEditController
    {
        // IMPORTANT: Uses the Proxy/Real concept - see https://r.2sxc.org/proxy-controllers

        public EditController() : base(EditControllerReal.LogSuffix) { }

        /// <inheritdoc />
        [HttpPost]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        [AllowAnonymous]   // will check security internally, so assume no requirements
        public EditDto Load([FromBody] List<ItemIdentifier> items, int appId) 
            => Log.Return(() => Real.Load(items, appId));

        /// <inheritdoc />
        [HttpPost]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        [AllowAnonymous] // will check security internally, so assume no requirements
        public Dictionary<Guid, int> Save([FromBody] EditDto package, int appId, bool partOfPage)
            => Log.Return(() => Real.Save(package, appId, partOfPage));


        /// <inheritdoc />
        [HttpGet]
        [HttpPost]
        [AllowAnonymous] // security check happens internally
        public IEnumerable<EntityForPickerDto> EntityPicker(
            [FromUri] int appId,
            [FromBody] string[] items,
            [FromUri] string contentTypeName = null)
            => Real.EntityPicker(appId, items, contentTypeName);

        /// <inheritdoc />
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public LinkInfoDto LinkInfo(string link, int appId, string contentType = default, Guid guid = default,
            string field = default)
            => Real.LinkInfo(link, appId, contentType, guid, field);

        /// <inheritdoc />
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public bool Publish(int id)
            => Real.Publish(id);
    }
}
