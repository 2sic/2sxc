using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.Types;
using static ToSic.Razor.Blade.Tag;

namespace ToSic.Sxc.Web.WebApi.System
{
    public partial class Insights
    {

        public string Types(int? appId = null, bool detailed = false)
        {
            ThrowIfNotSuperUser();
            if (appId == null)
                return "please add appid to the url parameters";

            Log.Add($"debug app types for {appId}");
            var appRead = AppRt(appId);
            var pkg = appRead.AppState;

            var msg = TypesTable(appId.Value, pkg.ContentTypes, pkg.List); //.ToList());

            return msg;
        }

        private string TypesTable(int appId, IEnumerable<IContentType> typesA, IReadOnlyCollection<IEntity> items)
        {
            var msg = H1($"App types for {appId}\n").ToString();
            try
            {
                Log.Add("getting content-type stats");
                var types = typesA
                    .OrderBy(t => t.RepositoryType)
                    .ThenBy(t => t.Scope)
                    .ThenBy(t => t.StaticName)
                    .ToList();
                msg += P($"types: {types.Count}\n");
                msg += "<table id='table'>"
                       + HeadFields(
                               "#", "Scope", "StaticName", "Name", "Attribs", "Metadata", "Permissions", "IsDyn",
                               "Repo", "Items"
                           )
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

                    msg = msg + RowFields(
                        ++count,
                        type.Scope,
                        type.StaticName,
                        type.Name,
                        A($"{type.Attributes.Count}").Href($"attributes?appid={appId}&type={type.StaticName}"),
                        A($"{type.Metadata.Count()}").Href($"typepermissions?appid={appId}&type={type.StaticName}"),
                        A($"{type.Metadata.Permissions.Count()}").Href(
                            $"typepermissions?appid={appId}&type={type.StaticName}"),
                        type.IsDynamic.ToString(),
                        type.RepositoryType.ToString(),
                        A($"{itemCount}").Href($"entities?appid={appId}&type={type.StaticName}")
                    );
                }
                msg += "</tbody>";
                msg += RowFields("", "", "", "", "", "", "", "", "",
                    A($"{totalItems}").Href($"entities?appid={appId}&type=all"));
                msg += "</table>";
                msg += "\n\n";
                msg += P(
                    $"Total item in system: {items?.Count} - in types: {totalItems} - numbers {Em("should")} match!");
                msg += JsTableSort();
            }
            catch
            {
                // ignored
            }

            return msg;
        }

        public string GlobalTypes()
        {
            ThrowIfNotSuperUser();

            var globTypes = Global.AllContentTypes().Values;
            return TypesTable(0, globTypes, null);
        }

        public string GlobalTypesLog()
        {
            ThrowIfNotSuperUser();
            return FormatLog("2sxc load log for Global Types", Global.Log);
        }

        public string TypeMetadata(int? appId = null, string type = null)
        {
            ThrowIfNotSuperUser();

            if (UrlParamsIncomplete(appId, type, out var message))
                return message;

            Log.Add($"debug app metadata for {appId} and {type}");
            var typ = AppState(appId).GetContentType(type);

            var msg = H1($"Metadata for {typ.Name} ({typ.StaticName}) in {appId}\n").ToString();
            var metadata = typ.Metadata.ToList();

            return MetadataTable(msg, metadata);
        }

        public string TypePermissions(int? appId = null, string type = null)
        {
            ThrowIfNotSuperUser();

            if (UrlParamsIncomplete(appId, type, out var message))
                return message;

            Log.Add($"debug app metadata for {appId} and {type}");
            var typ = AppState(appId).GetContentType(type);

            var msg = H1($"Permissions for {typ.Name} ({typ.StaticName}) in {appId}\n").ToString();
            var metadata = typ.Metadata.Permissions.Select(p => p.Entity).ToList();

            return MetadataTable(msg, metadata);
        }
    }
}
