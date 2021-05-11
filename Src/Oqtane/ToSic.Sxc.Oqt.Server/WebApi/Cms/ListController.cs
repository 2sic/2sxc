using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.WebApi.FieldList;

namespace ToSic.Sxc.Oqt.Server.WebApi.Cms
{
    // Release routes
    [Route(WebApiConstants.ApiRoot + "/cms/[controller]/[action]")]
    [Route(WebApiConstants.ApiRoot2 + "/cms/[controller]/[action]")]
    [Route(WebApiConstants.ApiRoot3 + "/cms/[controller]/[action]")]

    // Beta routes
    [Route(WebApiConstants.WebApiStateRoot + "/cms/[controller]/[action]")]

    //[ValidateAntiForgeryToken]
    //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    [Authorize(Roles = RoleNames.Admin)]
    public class ListController : OqtStatefulControllerBase
    {
        protected override string HistoryLogName => "Api.List";

        private readonly Lazy<FieldListBackend> _fieldListBackendLazy;
        private FieldListBackend FieldBacked => _fieldBackend ??= _fieldListBackendLazy.Value.Init(Log);
        private FieldListBackend _fieldBackend;
        public ListController(Lazy<FieldListBackend> fieldListBackendLazy)
        {
            _fieldListBackendLazy = fieldListBackendLazy;
        }

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