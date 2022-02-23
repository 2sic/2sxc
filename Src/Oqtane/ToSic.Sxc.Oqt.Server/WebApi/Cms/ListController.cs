using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Routing;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.WebApi.FieldList;

namespace ToSic.Sxc.Oqt.Server.WebApi.Cms
{
    // Release routes
    [Route(WebApiConstants.ApiRootWithNoLang + $"/{AreaRoutes.Cms}")]
    [Route(WebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Cms}")]
    [Route(WebApiConstants.ApiRootPathNdLang + $"/{AreaRoutes.Cms}")]

    // Beta routes - TODO: @STV - why is this beta?
    [Route(WebApiConstants.WebApiStateRoot + $"/{AreaRoutes.Cms}")]
    [ValidateAntiForgeryToken]
    //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    [Authorize(Roles = RoleNames.Admin)]
    public class ListController : OqtStatefulControllerBase<DummyControllerReal>
    {
        public ListController(Lazy<FieldListBackend> fieldListBackendLazy): base("List")
        {
            _fieldListBackendLazy = fieldListBackendLazy;
        }
        private readonly Lazy<FieldListBackend> _fieldListBackendLazy;
        private FieldListBackend FieldBacked => _fieldBackend ??= _fieldListBackendLazy.Value.Init(Log);
        private FieldListBackend _fieldBackend;

        /// <summary>
        /// used to be GET Module/ChangeOrder
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="fields"></param>
        /// <param name="index"></param>
        /// <param name="toIndex"></param>
        [HttpPost]
        public IActionResult Move(Guid? parent, string fields, int index, int toIndex)
        {
            FieldBacked.ChangeOrder(parent, fields, index, toIndex);
            return new NoContentResult();
        }

        /// <summary>
        /// Used to be Get Module/RemoveFromList
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="fields"></param>
        /// <param name="index"></param>
        [HttpDelete]
        public IActionResult Delete(Guid? parent, string fields, int index)
        {
            FieldBacked.Remove(parent, fields, index);
            return new NoContentResult();
        }
    }
}