using System;
using System.Collections.Generic;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav;
using ToSic.Sxc.WebApi.ItemLists;

namespace ToSic.Sxc.WebApi.Cms
{
    // TODO: these methods were once for ContentGroups only, now they work on every entity
    // Some day they should be moved to an own controller or to EntitiesController
    public partial class ContentGroupController
    {
        // TODO: shouldn't be part of ContentGroupController any more, as it's generic now
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public void Replace(Guid guid, string part, int index, int entityId, bool add = false)
            => Factory.Resolve<ListsBackendBase>().Init(GetBlock().App, Log)
                .Replace(GetContext(), guid, part, index, entityId, add);


        // TODO: WIP changing this from ContentGroup editing to any list editing
        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public dynamic Replace(Guid guid, string part, int index) 
            => Factory.Resolve<ListsBackendBase>().Init(GetBlock().App, Log)
                .GetReplacementOptions(guid, part, index);

        [HttpGet]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public List<EntityInListDto> ItemList(Guid guid, string part) 
            => Factory.Resolve<ListsBackendBase>().Init(GetBlock().App, Log)
                .ItemList(guid, part);


        // TODO: part should be handed in with all the relevant names! atm it's "content" in the content-block scenario
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
        public bool ItemList([FromUri] Guid guid, List<EntityInListDto> list, [FromUri] string part = null) 
            => Factory.Resolve<ListsBackendBase>().Init(GetBlock().App, Log)
                .Reorder(GetContext(), guid, list, part);
    }
}
