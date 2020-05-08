using System.Linq;
using System.Web.Http;
using ToSic.Eav.Data;

namespace ToSic.Sxc.WebApi.System
{
    public partial class InsightsController
    {

        [HttpGet]
        public string Attributes(int appId , string type = null)
        {
            ThrowIfNotSuperuser();

            if (UrlParamsIncomplete(appId, type, out var message))
                return message;

            Log.Add($"debug app attributes for {appId} and {type}");
            //var appRead = AppRt(appId);// new AppRuntime(appId, true, Log);
            var typ = AppState(appId).GetContentType(type);// appRead.ContentTypes.Get(type);
            //var pkg = appRead.Package;

            var msg = h1($"Attributes for {typ.Name} ({typ.StaticName}) in {appId}\n");
            try
            {
                Log.Add("getting content-type stats");
                var attribs = typ.Attributes;
                msg += p($"attribs: {attribs.Count}\n");
                msg += "<table id='table'><thead>" 
                    + tr(new[] { "#", "Id", "Name", "Type", "Input", "IsTitle", "Metadata", "Permissions" }, true)
                    + "</thead>"
                    + "<tbody>";
                var count = 0;
                foreach (var att in attribs)
                {
                    msg = msg + tr(new[] {
                        (++count).ToString(),
                        att.AttributeId.ToString(),
                        att.Name,
                        att.Type,
                        att.InputType(),
                        att.IsTitle.ToString(),
                        a($"{att.Metadata.Count()}", $"attributemetadata?appid={appId}&type={type}&attribute={att.Name}"),
                        a($"{att.Metadata.Permissions.Count()}", $"attributepermissions?appid={appId}&type={type}&attribute={att.Name}")
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
        public string AttributeMetadata(int? appId = null, string type = null, string attribute = null)
        {
            ThrowIfNotSuperuser();

            if (UrlParamsIncomplete(appId, type, attribute, out var message))
                return message;

            Log.Add($"debug app metadata for {appId} and {type}");
            //var appRead = AppRt(appId);// new AppRuntime(appId.Value, Log);
            var typ = AppState(appId.Value).GetContentType(type);// appRead.ContentTypes.Get(type);
            var att = typ.Attributes.First(a => a.Name == attribute)
                      ?? throw Http.BadRequest($"can't find attribute {attribute}");

            var msg = h1($"Attribute Metadata for {typ.Name}.{attribute} in {appId}\n");
            var metadata = att.Metadata.ToList();

            return MetadataTable(msg, metadata);
        }

        [HttpGet]
        public string AttributePermissions(int? appId = null, string type = null, string attribute = null)
        {
            if (UrlParamsIncomplete(appId, type, attribute, out var message))
                return message;

            Log.Add($"debug app metadata for {appId} and {type}");
            //var appRead = AppRt(appId);// new AppRuntime(appId.Value, Log);
            var typ = AppState(appId).GetContentType(type);//appRead.ContentTypes.Get(type);
            var att = typ.Attributes.First(a => a.Name == attribute)
                      ?? throw Http.BadRequest($"can't find attribute {attribute}");

            var msg = h1($"Attribute Permissions for {typ.Name}.{attribute} in {appId}\n");
            var metadata = att.Metadata.Permissions.Select(p => p.Entity).ToList();

            return MetadataTable(msg, metadata);
        }
    }
}