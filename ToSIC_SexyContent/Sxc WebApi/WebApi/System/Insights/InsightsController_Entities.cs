using System;
using System.Globalization;
using System.Linq;
using System.Web.Http;
using ToSic.Eav.Apps;
using ToSic.Eav.ImportExport.Json;
using IEntity = ToSic.Eav.Data.IEntity;

namespace ToSic.Sxc.WebApi.System
{
    public partial class InsightsController
    {

        [HttpGet]
        public string Entities(int? appId = null, string type = null)
        {
            ThrowIfNotSuperuser();

            if (UrlParamsIncomplete(appId, type, out var message))
                return message;

            Log.Add($"debug app attributes for {appId} and {type}");
            var appRead = AppRt(appId);// new AppRuntime(appId.Value, Log);
            
            var typ = appRead.ContentTypes.Get(type);
            //var pkg = appRead.Package;

            var msg = h1($"Entities for {type} ({typ?.Name}/{typ?.StaticName}) in {appId}\n");
            try
            {
                Log.Add("getting content-type stats");
                var entities = type == "all"
                    ? appRead.Entities.All.ToList()
                    : appRead.Entities.Get(type).ToList();
                msg += p($"entities: {entities.Count}\n");
                msg += "<table id='table'><thead>" 
                    + tr(new[] { "#", "Id", "Guid", "Title", "Type", "Modified", "Owner", "Version", "Metadata", "Permissions" }, true)
                    + "</thead>"
                    + "<tbody>";
                var count = 0;
                foreach (var ent in entities)
                {
                    msg = msg + tr(new[] {
                        (++count).ToString(),
                        a($"{ent.EntityId}", $"entity?appid={appId}&entity={ent.EntityId}"),
                        a($"{ent.EntityGuid}", $"entity?appid={appId}&entity={ent.EntityGuid}"),
                        ent.GetBestTitle(),
                        ent.Type.Name,
                        ent.Modified.ToString(CultureInfo.InvariantCulture),
                        ent.Owner,
                        $"{ent.Version}",
                        a($"{ent.Metadata.Count()}", $"entitymetadata?appid={appId}&entity={ent.EntityId}"),
                        a($"{ent.Metadata.Permissions.Count()}", $"entitypermissions?appid={appId}&entity={ent.EntityId}")
                          });
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

        [HttpGet]
        public string EntityMetadata(int? appId = null, int? entity = null)
        {
            ThrowIfNotSuperuser();

            if (UrlParamsIncomplete(appId, entity, out var message))
                return message;

            Log.Add($"debug app entity metadata for {appId} and entity {entity}");
            var appRead = AppRt(appId);// new AppRuntime(appId.Value, Log);
            var ent = appRead.Entities.Get(entity.Value);

            var msg = h1($"Entity Metadata for {entity} in {appId}\n");
            var metadata = ent.Metadata.ToList();

            return MetadataTable(msg, metadata);
        }

        [HttpGet]
        public string EntityPermissions(int? appId = null, int? entity = null)
        {
            ThrowIfNotSuperuser();

            if (UrlParamsIncomplete(appId, entity, out var message))
                return message;

            Log.Add($"debug app entity permissions for {appId} and entity {entity}");
            var appRead = AppRt(appId);// new AppRuntime(appId.Value, Log);
            var ent = appRead.Entities.Get(entity.Value);

            var msg = h1($"Entity Permissions for {entity} in {appId}\n");
            var permissions = ent.Metadata.Permissions.Select(p => p.Entity).ToList();

            return MetadataTable(msg, permissions);
        }

        [HttpGet]
        public string Entity(int? appId = null, string entity = null)
        {
            ThrowIfNotSuperuser();

            if (UrlParamsIncomplete(appId, entity, out var message))
                return message;

            Log.Add($"debug app entity metadata for {appId} and entity {entity}");
            var appRead = AppRt(appId);// new AppRuntime(appId.Value, Log);

            IEntity ent;
            if (Int32.TryParse(entity, out var entityId))
                ent = appRead.Entities.Get(entityId);
            else if (Guid.TryParse(entity, out var entityGuid))
                ent = appRead.Entities.Get(entityGuid);
            else
                throw Http.BadRequest("can't use entityid - must be number or guid");

            var ser = new JsonSerializer(appRead.AppState, Log);
            var json = ser.Serialize(ent);

            var msg = h1($"Entity Debug for {entity} in {appId}\n")
                      + ToBr("\n\n\n")
                      + Tag("textarea", json, " rows='20' cols='100' ");

            return msg;
        }

    }
}