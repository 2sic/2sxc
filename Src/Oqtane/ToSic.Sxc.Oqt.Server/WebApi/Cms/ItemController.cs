using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Routing;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.WebApi.InPage;

namespace ToSic.Sxc.Oqt.Server.WebApi.Cms
{
    // Release routes
    [Route(WebApiConstants.ApiRootWithNoLang + $"/{AreaRoutes.Cms}")]
    [Route(WebApiConstants.ApiRootPathOrLang + $"/{AreaRoutes.Cms}")]
    [Route(WebApiConstants.ApiRootPathNdLang + $"/{AreaRoutes.Cms}")]

    // Beta routes - TODO: @STV - why is this beta?
    [Route(WebApiConstants.WebApiStateRoot + $"/{AreaRoutes.Cms}")]
    [ValidateAntiForgeryToken]
    public class ItemController : OqtStatefulControllerBase<DummyControllerReal>
    {
        public ItemController(Lazy<AppViewPickerBackend> appViewPickerBackendLazy): base("Item")
        {
            _appViewPickerBackendLazy = appViewPickerBackendLazy;
        }
        private readonly Lazy<AppViewPickerBackend> _appViewPickerBackendLazy;

        /// <summary>
        /// Used to be GET Module/Publish
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        [Authorize(Roles = RoleNames.Admin)]
        // TODO: 2DM please check permissions
        public bool Publish(int id)
            => _appViewPickerBackendLazy.Value.Init(Log)
                .Publish(id);
    }
}