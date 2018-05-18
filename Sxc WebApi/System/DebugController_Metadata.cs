using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System.Linq;
using System.Web.Http;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.WebApi.System
{
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Host)]
    public partial class DebugController
    {



        private static string MetadataTable(string msg, ContentTypeMetadata metadata)
        {
            try
            {
                msg += p($"Metadata: {metadata.Count()}\n");
                msg += "<table id='table'><thead>"
                       + tr(new[] {"#", "Id", "Title", "Type", "Input", "MDTarget", "Key"}, true)
                       + "</thead>"
                       + "<tbody>";
                var count = 0;
                foreach (var md in metadata)
                {
                    var key = md.MetadataFor.KeyString + md.MetadataFor.KeyNumber + md.MetadataFor.KeyGuid;
                    if (string.IsNullOrEmpty(key))
                        key = "(directly attached)";
                    msg = msg + tr(new[]
                    {
                        (++count).ToString(),
                        md.EntityId.ToString(),
                        md.Type.Name,
                        md.GetBestTitle(),
                        md.MetadataFor.TargetType.ToString(),
                        key
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
    }
}