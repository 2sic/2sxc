using System.Collections.Generic;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System.Linq;
using System.Web.Http;
using ToSic.Eav.Apps;
using ToSic.Eav.Interfaces;
using ToSic.Eav.Types;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.WebApi.System
{
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
    public partial class InsightsController 
    {

        [HttpGet]
        public string Types(int? appId = null, bool detailed = false)
        {
            ThrowIfNotSuperuser();
            if (appId == null)
                return "please add appid to the url parameters";

            Log.Add($"debug app types for {appId}");
            var appRead = new AppRuntime(appId.Value, Log);
            var pkg = appRead.Package;

            var msg = TypesTable(appId.Value, pkg.ContentTypes, pkg.List.ToList());

            return msg;
        }

        private string TypesTable(int appId, IEnumerable<IContentType> typesA, List<IEntity> items)
        {
            var msg = h1($"App types for {appId}\n");
            try
            {
                Log.Add("getting content-type stats");
                var types = typesA
                    .OrderBy(t => t.RepositoryType)
                    .ThenBy(t => t.Scope)
                    .ThenBy(t => t.StaticName)
                    .ToList();
                msg += p($"types: {types.Count}\n");
                msg += "<table id='table'><thead>"
                       + tr(
                           new[]
                           {
                               "#", "Scope", "StaticName", "Name", "Attribs", "Metadata", "Permissions", "IsDyn",
                               "Repo", "Items"
                           }, true)
                       + "</thead>"
                       + "<tbody>";
                var totalItems = 0;
                var count = 0;
                foreach (var type in types)
                {
                    int? itemCount = null;
                    try
                    {
                        var itms = items?.Where(e => e.Type == type);
                        itemCount = itms?.Count();
                        totalItems += itemCount ?? 0;
                    }
                    catch
                    {
                        /*ignore*/
                    }
                    msg = msg + tr(new[]
                    {
                        (++count).ToString(),
                        type.Scope,
                        type.StaticName,
                        type.Name,
                        a($"{type.Attributes.Count}", $"attributes?appid={appId}&type={type.StaticName}"),
                        a($"{type.Metadata.Count()}", $"typepermissions?appid={appId}&type={type.StaticName}"),
                        a($"{type.Metadata.Permissions.Count()}",
                            $"typepermissions?appid={appId}&type={type.StaticName}"),
                        type.IsDynamic.ToString(),
                        type.RepositoryType.ToString(),
                        a($"{itemCount}", $"entities?appid={appId}&type={type.StaticName}")
                    });
                }
                msg += "</tbody>";
                msg += tr(
                    new[] {"", "", "", "", "", "", "", "", "", a($"{totalItems}", $"entities?appid={appId}&type=all")},
                    true);
                msg += "</table>";
                msg += "\n\n";
                msg += p(
                    $"Total item in system: {items?.Count} - in types: {totalItems} - numbers {em("should")} match!");
                msg += JsTableSort();
            }
            catch
            {
                // ignored
            }

            return msg;
        }

        [HttpGet]
        public string GlobalTypes()
        {
            var globTypes = Global.AllContentTypes().Values;
            return TypesTable(0, globTypes, null);
        }

        [HttpGet]
        public string GlobalTypesLog()
        {
            return ToBr(Global.Log.Dump(" - ", h1($"2sxc load log for global types") + "\n", "end of log"));
        }

        [HttpGet]
        public string TypeMetadata(int? appId = null, string type = null)
        {
            if (UrlParamsIncomplete(appId, type, out var message))
                return message;

            Log.Add($"debug app metadata for {appId} and {type}");
            var appRead = new AppRuntime(appId.Value, Log);
            var typ = appRead.ContentTypes.Get(type);

            var msg = h1($"Metadata for {typ.Name} ({typ.StaticName}) in {appId}\n");
            var metadata = typ.Metadata.ToList();

            return MetadataTable(msg, metadata);
        }

        [HttpGet]
        public string TypePermissions(int? appId = null, string type = null)
        {
            if (UrlParamsIncomplete(appId, type, out var message))
                return message;

            Log.Add($"debug app metadata for {appId} and {type}");
            var appRead = new AppRuntime(appId.Value, Log);
            var typ = appRead.ContentTypes.Get(type);

            var msg = h1($"Permissions for {typ.Name} ({typ.StaticName}) in {appId}\n");
            var metadata = typ.Metadata.Permissions.ToList();

            return MetadataTable(msg, metadata);
        }
        
    }
}