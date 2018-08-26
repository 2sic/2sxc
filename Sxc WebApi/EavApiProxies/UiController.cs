using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.Apps;
using ToSic.Eav.ImportExport.Json;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi;
using ToSic.Eav.WebApi.Formats;
using ToSic.SexyContent.WebApi.Errors;
using ToSic.SexyContent.WebApi.Permissions;

namespace ToSic.SexyContent.WebApi.EavApiProxies
{
    [SupportedModules("2sxc,2sxc-app")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)] // while in dev-mode, only for super-users
    public class UiController : SxcApiControllerBase
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
            if(!permCheck.EnsureAll(GrantSets.WriteSomething, items, out var exp))
                throw exp;
            permCheck.InitAppData();

            // load items - similar
            var result = new AllInOne();
            var entityApi = new EntityApi(appId, Log);
            var typeRead = entityApi.AppManager.Read.ContentTypes;
            items = EntitiesController.ConvertListIndexToId(items, permCheck.App);
            var list = entityApi.GetEntitiesForEditing(appId, items);
            result.Items = list.Select(e => new EntityWithHeader2
            {
                Header = e.Header,
                Entity = JsonSerializer.ToJson(e.Entity)
            }).ToList();

            // set published if some data already exists
            if (list.Any())
                result.IsPublished = list.First().Entity?.IsPublished ?? true; // Entity could be null (new), then true

            // load content-types
            var types = list.Select(i
                    // try to get the entity type, but if there is none (new), look it up according to the header
                    => i.Entity?.Type
                       ?? typeRead.Get(i.Header.ContentTypeName))
                .ToList();
            result.ContentTypes = types.Select(JsonSerializer.ToJson).ToList();

            // load input-field configurations
            var fields = types.SelectMany(t => t.Attributes).Select(a => a.InputTypeTempBetterForNewUi).Distinct();
            var appInputTypes = typeRead.GetInputTypes();
            result.InputTypes = appInputTypes.Where(it => fields.Contains(it.Type)).ToList();

            // done - return
            return result;
        }

        [HttpPost]
        public string Save([FromBody] AllInOne package, int appId, bool partOfPage)
        {
            // perform some basic validation checks
            if (package.ContentTypes != null)
                throw Http.BadRequest("package contained content-types, unexpected");
            if (package.InputTypes != null)
                throw Http.BadRequest("package contained input types, unexpected");
            if (package.Items == null || package.Items.Count == 0)
                throw Http.BadRequest("package didn't contain items, unexpected");

            var appMan = new AppRuntime(appId, Log);
            var ser = new JsonSerializer(appMan.Package, Log);

            var count = 0;
            foreach (var item in package.Items)
            {
                if (item.Header == null || item.Entity == null)
                    throw Http.BadRequest($"item {count} header or entity is missing");

                var ent = ser.Deserialize(item.Entity, false, false);
                if (ent == null)
                    throw Http.BadRequest($"entity {count} couldn't deserialize");
                if (ent.Attributes.Count == 0)
                    throw Http.BadRequest($"entity {count} doesn't have attributes (or they are invalid)");

                var original = appMan.Entities.Get(ent.EntityId);
                if (original != null && original.Attributes.Count != ent.Attributes.Count)
                    throw Http.BadRequest(
                        $"entity {count} has different amount of attributes {ent.Attributes.Count} than the original {original.Attributes.Count}");

                count++;
            }

            return "call to save worked - save doesn't work yet";
        }
    }
}
