using System.Linq;
using ToSic.Eav.Data;
using static ToSic.Razor.Blade.Tag;

namespace ToSic.Sxc.Web.WebApi.System
{
    public partial class Insights
    {

        private string Attributes(int? appId, string type)
        {
            if (UrlParamsIncomplete(appId, type, out var message))
                return message;

            Log.Add($"debug app attributes for {appId} and {type}");
            var typ = AppState(appId).GetContentType(type);

            var msg = "" + H1($"Attributes for {typ.Name} ({typ.NameId}) in {appId}\n");
            try
            {
                Log.Add("getting content-type stats");
                var attribs = typ.Attributes;
                msg += P($"attribs: {attribs.Count}\n");
                msg += "<table id='table'>"
                    + HeadFields( "#", "Id", "Name", "Type", "Input", "IsTitle", "Metadata", "Permissions" )
                    + "<tbody>";
                var count = 0;
                foreach (var att in attribs)
                {
                    msg += RowFields(
                        (++count).ToString(),
                        att.AttributeId.ToString(),
                        att.Name,
                        att.Type,
                        att.InputType(),
                        EmojiTrueFalse(att.IsTitle),
                        LinkTo($"{att.Metadata.Count()}", nameof(AttributeMetadata), appId, type: type, nameId: att.Name),
                        LinkTo($"{att.Metadata.Permissions.Count()}", nameof(AttributePermissions), appId, type: type, nameId: att.Name)
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

        private string AttributeMetadata(int? appId, string type, string nameId)
        {
            if (UrlParamsIncomplete(appId, type, nameId, out var message))
                return message;

            Log.Add($"debug app metadata for {appId} and {type}");
            var typ = AppState(appId.Value).GetContentType(type);
            var att = typ.Attributes.First(a => a.Name == nameId)
                      ?? throw CreateBadRequest($"can't find attribute {nameId}");

            var msg = H1($"Attribute Metadata for {typ.Name}.{nameId} in {appId}\n").ToString();
            var metadata = att.Metadata.ToList();

            return MetadataTable(msg, metadata);
        }

        private string AttributePermissions(int? appId, string type, string attribute)
        {
            if (UrlParamsIncomplete(appId, type, attribute, out var message))
                return message;

            Log.Add($"debug app metadata for {appId} and {type}");
            var typ = AppState(appId).GetContentType(type);
            var att = typ.Attributes.First(a => a.Name == attribute)
                      ?? throw CreateBadRequest($"can't find attribute {attribute}");

            var msg = H1($"Attribute Permissions for {typ.Name}.{attribute} in {appId}\n").ToString();
            var metadata = att.Metadata.Permissions.Select(p => p.Entity).ToList();

            return MetadataTable(msg, metadata);
        }
    }
}
