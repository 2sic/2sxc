using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using System;
using ToSic.Eav.WebApi.Cms;
using ToSic.Eav.WebApi.Routing;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.WebApi.Cms;

namespace ToSic.Sxc.Oqt.Server.WebApi.Cms
{
    // Release routes
    [Route(WebApiConstants.ApiRootWithNoLang + $"/{AreaRoutes.Cms}")]
    [Route(WebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Cms}")]
    [Route(WebApiConstants.ApiRootPathNdLang + $"/{AreaRoutes.Cms}")]

    [ValidateAntiForgeryToken]
    //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    [Authorize(Roles = RoleNames.Admin)]
    public class ListController : OqtStatefulControllerBase<ListControllerReal>, IListController
    {
        public ListController(): base(ListControllerReal.LogSuffix) { }

        /// <summary>
        /// used to be GET Module/ChangeOrder
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="fields"></param>
        /// <param name="index"></param>
        /// <param name="toIndex"></param>
        [HttpPost]
        public void Move(Guid? parent, string fields, int index, int toIndex) 
            => Real.Move(parent, fields, index, toIndex);

        /// <summary>
        /// Used to be Get Module/RemoveFromList
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="fields"></param>
        /// <param name="index"></param>
        [HttpDelete]
        public void Delete(Guid? parent, string fields, int index) 
            => Real.Delete(parent, fields, index);
    }
}