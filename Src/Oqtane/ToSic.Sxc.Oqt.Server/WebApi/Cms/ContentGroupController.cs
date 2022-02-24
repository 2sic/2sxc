using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Routing;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.WebApi.ItemLists;

namespace ToSic.Sxc.Oqt.Server.WebApi.Cms
{
    // Release routes
    [Route(WebApiConstants.ApiRootWithNoLang + $"/{AreaRoutes.Cms}")]
    [Route(WebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Cms}")]
    [Route(WebApiConstants.ApiRootPathNdLang + $"/{AreaRoutes.Cms}")]

    [ValidateAntiForgeryToken]
    [ApiController]
    // cannot use this, as most requests now come from a lone page [SupportedModules("2sxc,2sxc-app")]
    public class ContentGroupController : OqtStatefulControllerBase<DummyControllerReal>
    {
        public ContentGroupController(Lazy<ContentGroupControllerReal> listBackendLazy): base("ConGrp")
        {
            _listBackendLazy = listBackendLazy;
        }
        private readonly Lazy<ContentGroupControllerReal> _listBackendLazy;

        private ContentGroupControllerReal Backend => _listBackendLazy.Value.Init(Log);


        [HttpGet]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [Authorize(Roles = RoleNames.Admin)]
        public EntityInListDto Header(Guid guid)
            => Backend.HeaderItem(guid);


        // TODO: shouldn't be part of ContentGroupController any more, as it's generic now
        [HttpPost]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [Authorize(Roles = RoleNames.Admin)]
        public void Replace(Guid guid, string part, int index, int entityId, bool add = false)
            => Backend.Replace(guid, part, index, entityId, add);


        // TODO: WIP changing this from ContentGroup editing to any list editing
        [HttpGet]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [Authorize(Roles = RoleNames.Admin)]
        public dynamic Replace(Guid guid, string part, int index)
            => Backend.GetReplacementOptions(guid, part, index);

        [HttpGet]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [Authorize(Roles = RoleNames.Admin)]
        public List<EntityInListDto> ItemList(Guid guid, string part)
            => Backend.ItemList(guid, part);


        // TODO: part should be handed in with all the relevant names! atm it's "content" in the content-block scenario
        [HttpPost]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        [Authorize(Roles = RoleNames.Admin)]
        public bool ItemList([FromQuery] Guid guid, List<EntityInListDto> list, [FromQuery] string part = null)
            => Backend.Reorder(guid, list, part);

    }
}