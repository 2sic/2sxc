using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.WebApi.PublicApi;
using ToSic.Sxc.WebApi;
using ToSic.Sxc.WebApi.InPage;

namespace ToSic.Sxc.Dnn.WebApi.Cms
{
    [ValidateAntiForgeryToken]
    [SupportedModules("2sxc,2sxc-app")]
    public class ItemController : SxcApiControllerBase, IItemController
    {
        public ItemController() : base("Item") { }

        /// <inheritdoc />
        [HttpPost]
        [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
        public bool Publish(int id)
            => GetService<AppViewPickerBackend>().Init(Log).Publish(id);
    }
}