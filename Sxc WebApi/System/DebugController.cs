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
    public partial class DebugController : DnnApiControllerWithFixes
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext); // very important!!!
            Log.Rename("Api.Debug");
        }

        [HttpGet]
        public string Help()
        {
            return h1("Commands")
                   + "<ol>"
                   + li(a("cache", "cache"))
                   + "<li><a href='help'>help</a></li>"
                   + "<li><a href='isalive'>isalive</a></li>\n"
                   + "<li><a href='loadlog?appid='>loadlog?appid=</a></li>\n"
                   + "<li><a href='stats?appid='>stats?appid=</a></li>\n"
                   + "<li><a href='types?appid='>types?appid=</a></li>\n"
                   + "<li><a href='types?appId=&detailed=true'>types?appId= &detailed=true</a></li>"
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
            ThrowIfNotSuperuser();
            if (appId == null)
                return "please add appid to the url parameters";

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
                + tr(new []{"Zone", "App", "InCache", "Details", "Actions"}, true)
                + "</thead>"
                + "<tbody>";
            foreach (var zone in zones)
            {
                var apps = zone.Value.Apps
                    .Select(a => new { Id = a.Key, InCache = cache.HasCacheItem(zone.Value.ZoneId, a.Key)})
                    .OrderBy(a => a.Id);
                foreach (var app in apps)
                {
                    msg += tr(new[]
                    {
                        zone.Key.ToString(),
                        app.Id.ToString(),
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
            ThrowIfNotSuperuser();
            if (appId == null)
                return "please add appid to the url parameters";

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