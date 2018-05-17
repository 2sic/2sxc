using System.Linq;
using System.Web.Http;
using System.Web.Http.Controllers;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.DataSources.Caches;
using ToSic.SexyContent.WebApi.Dnn;
using ToSic.SexyContent.WebApi.Errors;

namespace ToSic.SexyContent.WebApi
{
    [SxcWebApiExceptionHandling]
    [AllowAnonymous]
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

            var zones = cache.ZoneApps.GroupBy(z => z.Value).OrderBy(z => z.Key.ZoneId);

            msg += "<ol>";
            foreach (var zone in zones)
            {
                var aList = "<ol>";
                var apps = zone.Key.Apps.Select(a => a.Key).OrderBy(a => a);
                foreach (var app in apps)
                {
                    aList += li($"App: {app} - {a("stats", $"stats?appid={app}")} | {a("load log", $"loadlog?appid={app}")} | {a("types", $"types?appid={app}")}");
                }
                aList += "</ol>";

                msg += li($"Zone:{zone.Key.ZoneId} Default App:{zone.Key.DefaultAppId} {aList}");
            }
            msg += "</ol>";
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


        [HttpGet]
        public string Types(int? appId = null, bool detailed = false)
        {
            ThrowIfNotSuperuser();
            if (appId == null)
                return "please add appid to the url parameters";

            Log.Add($"debug app types for {appId}");
            var appRead = new AppRuntime(appId.Value, Log);
            var pkg = appRead.Package;

            var msg = h1($"App types for {appId}\n");
            try
            {
                Log.Add("getting content-type stats");
                var types = pkg.ContentTypes.ToList();
                msg += p($"types: {types.Count}\n");
                msg += "<ol>";
                foreach (var type in types)
                {
                    int? itemCount = null;
                    try
                    {
                        var itms = pkg.List.Where(e => e.Type == type);
                        itemCount = itms.Count();
                    }
                    catch {  /*ignore*/ }
                    msg = msg + li($"Type: {type.Scope}.{type.StaticName} ({type.Name}) " +
                          $"with {type.Attributes.Count} attribs " +
                          $"- dyn:{type.IsDynamic} glob:{type.RepositoryType} items:{itemCount}\n");
                }

                msg += "</ol>\n\n";
            }
            catch
            {
                // ignored
            }

            return msg;
        }
    }
}