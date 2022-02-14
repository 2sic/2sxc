using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.Persistence.File;
using static ToSic.Razor.Blade.Tag;

namespace ToSic.Sxc.Web.WebApi.System
{
    public partial class Insights
    {
        private string Types(int? appId = null)
        {
            if (appId == null)
                return "please add appid to the url parameters";

            Log.Add($"debug app types for {appId}");
            var appRead = AppRt(appId);
            var pkg = appRead.AppState;

            var msg = TypesTable(appId.Value, pkg.ContentTypes, pkg.List);

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
                    .ThenBy(t => t.NameId)
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
                        type.NameId,
                        type.Name,
                        LinkTo($"{type.Attributes.Count}", nameof(Attributes), appId, type: type.NameId),
                        LinkTo($"{type.Metadata.Count()}", nameof(TypeMetadata), appId, type: type.NameId),
                        LinkTo($"{type.Metadata.Permissions.Count()}", nameof(TypePermissions), appId, type: type.NameId),
                        type.IsDynamic.ToString(),
                        type.RepositoryType.ToString(),
                        LinkTo($"{itemCount}", nameof(Entities), appId, type: type.NameId)
                    );
                }
                msg += "</tbody>";
                msg += RowFields("", "", "", "", "", "", "", "", "",
                    LinkTo($"{totalItems}", nameof(Entities), appId, type: "all"));
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

        private string GlobalTypes()
        {
            var globTypes = _appStates.GetPresetApp().ContentTypes;
            return TypesTable(Eav.Constants.PresetAppId, globTypes, null);
        }

        private string GlobalTypesLog()
        {
            var msg = PageStyles() + LogHeader();
            var log = Runtime.LoadLog;
            return msg + (log == null
                ? P("log is null").ToString()
                : DumpTree($"Log for Global Types loading", log));
        }

        private string TypeMetadata(int? appId = null, string type = null)
        {
            if (UrlParamsIncomplete(appId, type, out var message))
                return message;

            Log.Add($"debug app metadata for {appId} and {type}");
            var typ = AppState(appId).GetContentType(type);

            var msg = H1($"Metadata for {typ.Name} ({typ.NameId}) in {appId}\n").ToString();
            var metadata = typ.Metadata.ToList();

            return MetadataTable(msg, metadata);
        }

        private string TypePermissions(int? appId = null, string type = null)
        {
            if (UrlParamsIncomplete(appId, type, out var message))
                return message;

            Log.Add($"debug app metadata for {appId} and {type}");
            var typ = AppState(appId).GetContentType(type);

            var msg = H1($"Permissions for {typ.Name} ({typ.NameId}) in {appId}\n").ToString();
            var metadata = typ.Metadata.Permissions.Select(p => p.Entity).ToList();

            return MetadataTable(msg, metadata);
        }
    }
}
