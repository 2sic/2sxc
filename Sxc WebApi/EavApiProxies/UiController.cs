using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.ImportExport.Json;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Formats;
using ToSic.SexyContent.WebApi.Permissions;

namespace ToSic.SexyContent.WebApi.EavApiProxies
{
    [SupportedModules("2sxc,2sxc-app")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)] // while in dev-mode, only for super-users
    public class UiController : SxcApiController
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext); // very important!!!
            Log.Rename("Api.UiCont");
        }


        [HttpPost]
        public AllInOne Load([FromBody] List<ItemIdentifier> items, int appId)
        {
            // Security check
            Log.Add($"load many a#{appId}, items⋮{items.Count}");

            // to do full security check, we'll have to see what content-type is requested
            var permCheck = new AppAndPermissions(SxcInstance, appId, Log);
            permCheck.EnsureOrThrow(GrantSets.WriteSomething, items);
            permCheck.InitAppData();

            // load items - similar
            var result = new AllInOne();
            var entityApi = new EntityApi(appId, Log);
            items = EntitiesController.ConvertListIndexToId(items, permCheck.App);
            var list = entityApi.GetEntitiesForEditing(appId, items);
            result.Items = list.Select(e => new EntityWithHeader2
            {
                Header = e.Header,
                Entity = JsonSerializer.ToJson(e.Entity)
            }).ToList();

            // load content-types
            var types = list.Select(i => i.Entity.Type).ToList();
            result.ContentTypes = types.Select(JsonSerializer.ToJson).ToList();

            // load input-field configurations
            var fields = types.SelectMany(t => t.Attributes).Select(a => a.InputType).Distinct();
            var appInputTypes = entityApi.AppManager.Read.ContentTypes.GetInputTypes();
            result.InputTypes = appInputTypes.Where(it => fields.Contains(it.Type)).ToList();

            // done - return
            return result;
        }
    }
}
