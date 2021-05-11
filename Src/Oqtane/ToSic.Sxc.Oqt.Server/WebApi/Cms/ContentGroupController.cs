using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.WebApi.ItemLists;

namespace ToSic.Sxc.Oqt.Server.WebApi.Cms
{
    // Release routes
    [Route(WebApiConstants.ApiRoot + "/cms/[controller]/[action]")]
    [Route(WebApiConstants.ApiRoot2 + "/cms/[controller]/[action]")]
    [Route(WebApiConstants.ApiRoot3 + "/cms/[controller]/[action]")]

    // Beta routes
    [Route(WebApiConstants.WebApiStateRoot + "/cms/[controller]/[action]")]

    [ValidateAntiForgeryToken]
    [ApiController]
    // cannot use this, as most requests now come from a lone page [SupportedModules("2sxc,2sxc-app")]
    public class ContentGroupController : OqtStatefulControllerBase
    {
        private readonly Lazy<ListsBackendBase> _listBackendLazy;
        protected override string HistoryLogName => "Api.ConGrp";
        public ContentGroupController(Lazy<ListsBackendBase> listBackendLazy)
        {
            _listBackendLazy = listBackendLazy;
        }

        private ListsBackendBase Backend => _listBackendLazy.Value.Init(Log);


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