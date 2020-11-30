using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.WebApi.InPage;

namespace ToSic.Sxc.Oqt.Server.Controllers
{
    [Route(WebApiConstants.WebApiStateRoot + "/cms/[controller]/[action]")]
    [ValidateAntiForgeryToken]
    public class ItemController : OqtStatefulControllerBase
    {
        private readonly Lazy<AppViewPickerBackend> _appViewPickerBackendLazy;
        protected override string HistoryLogName => "Api.Item";

        public ItemController(StatefulControllerDependencies dependencies,
            Lazy<AppViewPickerBackend> appViewPickerBackendLazy) : base(dependencies)
        {
            _appViewPickerBackendLazy = appViewPickerBackendLazy;
        }

        /// <summary>
        /// Used to be GET Module/Publish
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        //[Authorize(Policy = "ViewModule")] // TODO: disabled
        public bool Publish(int id)
            => _appViewPickerBackendLazy.Value.Init(GetContext(), GetBlock(), Log)
                .Publish(id);
    }
}