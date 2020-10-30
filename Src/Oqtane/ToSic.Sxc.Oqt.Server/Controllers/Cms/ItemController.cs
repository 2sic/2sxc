using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sxc.WebApi.InPage;

namespace ToSic.Sxc.Oqt.Server.Controllers
{
    [Route(WebApiConstants.WebApiStateRoot + "/cms/[controller]/[action]")]
    [ValidateAntiForgeryToken]
    public class ItemController : SxcStatefulControllerBase
    {
        protected override string HistoryLogName => "Api.Item";

        public ItemController(StatefulControllerDependencies dependencies) : base(dependencies)
        {
        }

        /// <summary>
        /// Used to be GET Module/Publish
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        //[Authorize(Policy = "ViewModule")] // TODO: disabled
        public bool Publish(int id)
            => new AppViewPickerBackend().Init(GetContext(), GetBlock(), Log)
                .Publish(id);
    }
}