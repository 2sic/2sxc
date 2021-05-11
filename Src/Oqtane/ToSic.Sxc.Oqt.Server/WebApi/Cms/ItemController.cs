using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Oqtane.Shared;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.WebApi.InPage;

namespace ToSic.Sxc.Oqt.Server.WebApi.Cms
{
    // Release routes
    [Route(WebApiConstants.ApiRoot + "/cms/[controller]/[action]")]
    [Route(WebApiConstants.ApiRoot2 + "/cms/[controller]/[action]")]
    [Route(WebApiConstants.ApiRoot3 + "/cms/[controller]/[action]")]

    // Beta routes
    [Route(WebApiConstants.WebApiStateRoot + "/cms/[controller]/[action]")]

    //[ValidateAntiForgeryToken]
    public class ItemController : OqtStatefulControllerBase
    {
        private readonly Lazy<AppViewPickerBackend> _appViewPickerBackendLazy;
        protected override string HistoryLogName => "Api.Item";

        public ItemController(Lazy<AppViewPickerBackend> appViewPickerBackendLazy)
        {
            _appViewPickerBackendLazy = appViewPickerBackendLazy;
        }

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