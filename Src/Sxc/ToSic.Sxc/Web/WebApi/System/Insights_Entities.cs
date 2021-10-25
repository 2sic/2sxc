using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.ImportExport.Json;
using ToSic.Eav.Plumbing;
using ToSic.Razor.Blade;
using static ToSic.Razor.Blade.Tag;

namespace ToSic.Sxc.Web.WebApi.System
{
    public partial class Insights
    {

        public string Entities(int? appId = null, string type = null)
        {
            ThrowIfNotSuperUser();

            if (UrlParamsIncomplete(appId, type, out var message))
                return message;

            Log.Add($"debug app attributes for {appId} and {type}");
            var appRead = AppRt(appId);

            var typ = appRead.ContentTypes.Get(type);

            var msg = "" + H1($"Entities for {type} ({typ?.Name}/{typ?.StaticName}) in {appId}\n");
            try
            {
                Log.Add("getting content-type stats");
                var entities = type == "all"
                    ? appRead.Entities.All.ToImmutableArray()
                    : appRead.Entities.Get(type).ToImmutableArray();
                msg += P($"entities: {entities.Length}\n");
                msg += "<table id='table'>"
                    + HeadFields("#", "Id", Eav.Data.Attributes.GuidNiceName, Eav.Data.Attributes.TitleNiceName, "Type", "Modified", "Owner", "Version", "Metadata", "Permissions")
                    + "<tbody>";
                var count = 0;
                foreach (var ent in entities)
                {
                    msg = msg + RowFields(
                        (++count).ToString(),
                        A($"{ent.EntityId}").Href($"entity?appid={appId}&entity={ent.EntityId}"),
                        A($"{ent.EntityGuid}").Href($"entity?appid={appId}&entity={ent.EntityGuid}"),
                        ent.GetBestTitle(),
                        ent.Type.Name,
                        ent.Modified.ToString(CultureInfo.InvariantCulture),
                        ent.Owner,
                        $"{ent.Version}",
                        A($"{ent.Metadata.Count()}").Href($"entitymetadata?appid={appId}&entity={ent.EntityId}"),
                        A($"{ent.Metadata.Permissions.Count()}")
                            .Href($"entitypermissions?appid={appId}&entity={ent.EntityId}")
                    );
                }
                msg += "</tbody>";
                msg += "</table>";
                msg += "\n\n";
                msg += JsTableSort();
            }
            catch
            {
                // ignored
            }

            return msg;
        }

        public string EntityMetadata(int? appId = null, int? entity = null)
        {
            ThrowIfNotSuperUser();

            if (UrlParamsIncomplete(appId, entity, out var message))
                return message;

            Log.Add($"debug app entity metadata for {appId} and entity {entity}");
            var appRead = AppRt(appId);
            var ent = appRead.Entities.Get(entity.Value);

            var msg = H1($"Entity Metadata for {entity} in {appId}\n").ToString();
            var metadata = ent.Metadata.ToList();

            return MetadataTable(msg, metadata);
        }

        public string EntityPermissions(int? appId = null, int? entity = null)
        {
            ThrowIfNotSuperUser();

            if (UrlParamsIncomplete(appId, entity, out var message))
                return message;

            Log.Add($"debug app entity permissions for {appId} and entity {entity}");
            var appRead = AppRt(appId);
            var ent = appRead.Entities.Get(entity.Value);

            var msg = H1($"Entity Permissions for {entity} in {appId}\n").ToString();
            var permissions = ent.Metadata.Permissions.Select(p => p.Entity).ToList();

            return MetadataTable(msg, permissions);
        }

        public string Entity(int? appId = null, string entity = null)
        {
            ThrowIfNotSuperUser();

            if (UrlParamsIncomplete(appId, entity, out var message))
                return message;

            Log.Add($"debug app entity metadata for {appId} and entity {entity}");
            var appRead = AppRt(appId);

            IEntity ent;
            if (Int32.TryParse(entity, out var entityId))
                ent = appRead.Entities.Get(entityId);
            else if (Guid.TryParse(entity, out var entityGuid))
                ent = appRead.Entities.Get(entityGuid);
            else
                throw CreateBadRequest("can't use entityid - must be number or guid");

            var ser = _serviceProvider.Build<JsonSerializer>().Init(appRead.AppState, Log);
            var json = ser.Serialize(ent);

            var msg = H1($"Entity Debug for {entity} in {appId}\n")
                      + Tags.Nl2Br("\n\n\n")
                      + Textarea(json).Rows("20").Cols("100");

            return msg;
        }

    }
}
