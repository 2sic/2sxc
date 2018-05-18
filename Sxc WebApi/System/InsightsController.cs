using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.DataSources.Caches;
using ToSic.SexyContent.WebApi;
using ToSic.SexyContent.WebApi.Dnn;
using ToSic.SexyContent.WebApi.Errors;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.WebApi.System
{
    [SxcWebApiExceptionHandling]
    [AllowAnonymous]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
    public partial class InsightsController : DnnApiControllerWithFixes
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext); // very important!!!
            Log.Rename("Api.Debug");
        }

        [HttpGet]
        public string Help()
        {
            const string typeattribs = "typeattributes?appid=&type=";
            const string typeMeta = "typemetadata?appid=&type=";
            const string typePerms = "typepermissions?appid=&type=";
            const string attribMeta = "attributemetadata?appid=&type=&attribute=";
            const string attribPerms = "attributepermissions?appid=&type=&attribute=";
            const string entities = "entities?appid=&type=";
            const string entitiesAll = "entities?appid=&type=all";
            const string entity = "entity?appId=&entity=";
            const string entityMeta = "entitymetadata?appid=&entity=";
            const string entityPerms = "entitypermissions?appid=&entity=";
            const string purgeapp = "purge?appid=";
            const string globTypes = "globaltypes";
            const string globTypesLog = "globaltypeslog";
            return h1("2sxc Insights - Commands")
                   + p(
                       "In most cases you'll just browse the cache and use the links from there. "
                       + "The other links are listed here so you know what they would be, "
                       + "in case something is preventing you from browsing the normal way. "
                       + "Read more about 2sxc insights in the "
                       + a("blog post", "https://2sxc.org/en/blog/post/using-2sxc-insights", true))
                   + "<ol>"
                   + li("browse the in-memory " + a("cache", "cache"))
                   + "<li>look at <a href='help'>help</a> (this screen)</li>"
                   + "<li>ping the system to see if it's alive <a href='isalive'>isalive</a></li>\n"
                   + "<li>look at the load-log of an app-cache: <a href='loadlog?appid='>loadlog?appid=</a></li>\n"
                   + "<li>look at the cache-stats of an app: <a href='stats?appid='>stats?appid=</a></li>\n"
                   + li("flush an app cache: " + a(purgeapp, purgeapp))
                   + li("global types in cache: " + a(globTypes, globTypes))
                   + li("global types loading log: " + a(globTypesLog, globTypesLog))
                   + "<li>look at the content-types of an app: <a href='types?appid='>types?appid=</a></li>\n"
                   + li("look at attributes of a type: " + a(typeattribs, typeattribs))
                   + li("look at type metadata:" + a(typeMeta, typeMeta))
                   + li("look at type permissions:" + a(typePerms, typePerms)) 
                   + li("look at attribute Metadata :" + a(attribMeta, attribMeta)) 
                   + li("look at attribute permissions:" + a(attribPerms, attribPerms))
                   + li("look at entities of a type:" + a(entities, entities))
                   + li("look at all entities:" + a(entitiesAll, entitiesAll))
                   + li("look at a sigle entity by id:" + a(entity, entity))
                   + li("look at entity metadata using entity-id:" + a(entityMeta, entityMeta))
                   + li("look at entity permissions using entity-id:" + a(entityPerms, entityPerms))
                   + "<ol>"
                ;
        }

        private void ThrowIfNotSuperuser()
        {
             if (!PortalSettings.UserInfo.IsSuperUser)
                throw Http.PermissionDenied("requires Superuser permissions");
        }

        [HttpGet]
        public bool IsAlive()
        {
            ThrowIfNotSuperuser();
            return true;
        }

        [HttpGet]
        public string LoadLog(int? appId = null)
        {
            if (UrlParamsIncomplete(appId, out var message))
                return message;

            Log.Add($"debug app-load {appId}");
            var appRead = new AppRuntime(appId.Value, Log);
            return ToBr(appRead.Package.Log.Dump(" - ", h1($"2sxc load log for app {appId}") + "\n", "end of log"));
        }

        [HttpGet]
        public string Cache()
        {
            ThrowIfNotSuperuser();

            var msg = h1("Apps In Cache");
            var cache = (BaseCache) DataSource.GetCache(null);

            var zones = cache.ZoneApps.OrderBy(z => z.Key);

            msg += "<table id='table'><thead>"
                + tr(new []{"Zone", "App", "Guid", "InCache", "Details", "Actions"}, true)
                + "</thead>"
                + "<tbody>";
            foreach (var zone in zones)
            {
                var apps = zone.Value.Apps
                    .Select(a => new { Id = a.Key, Guid = a.Value, InCache = cache.HasCacheItem(zone.Value.ZoneId, a.Key)})
                    .OrderBy(a => a.Id);
                foreach (var app in apps)
                {
                    msg += tr(new[]
                    {
                        zone.Key.ToString(),
                        app.Id.ToString(),
                        $"{app.Guid}",
                        app.InCache ? "yes" : "no",
                        $"{a("stats", $"stats?appid={app.Id}")} | {a("load log", $"loadlog?appid={app.Id}")} | {a("types", $"types?appid={app.Id}")}",
                        summary("show actions",
                            $"{a("purge", $"purge?appid={app.Id}")}"
                        )
                    });
                }
            }
            msg += "</tbody>"
                   + "</table>"
                   + JsTableSort();
            return msg;
        }

        [HttpGet]
        public string Stats(int? appId = null)
        {
            if (UrlParamsIncomplete(appId, out var message))
                return message;

            Log.Add($"debug app-internals for {appId}");
            var appRead = new AppRuntime(appId.Value, Log);
            var pkg = appRead.Package;

            var msg = h1($"App internals for {appId}");
            try
            {
                Log.Add("general stats");
                msg += p(
                    ToBr($"AppId: {pkg.AppId}\n"
                         + $"Timestamp: {pkg.CacheTimestamp}\n"
                         + $"Update Count: {pkg.CacheUpdateCount}\n"
                         + $"Dyn Update Count: {pkg.DynamicUpdatesCount}\n"
                         + "\n")
                );
            }
            catch { /* ignore */ }

            return msg;
        }

    }
}