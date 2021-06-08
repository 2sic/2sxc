using System.Collections.Generic;
using ToSic.Eav.Data;

namespace ToSic.Sxc.Web.WebApi.System
{
    public partial class Insights
    {

        internal static string MetadataTable(string msg, List<IEntity> metadata)
        {
            try
            {
                msg += p($"Assigned Items: {metadata.Count}\n");
                msg += "<table id='table'><thead>"
                       + tr(new[] { "#", "Id", Eav.Data.Attributes.TitleNiceName, "Content-Type", "Target", "Key" }, true)
                       + "</thead>"
                       + "<tbody>";
                var count = 0;
                foreach (var md in metadata)
                {
                    var mdFor = md.MetadataFor;
                    var key = !string.IsNullOrEmpty(mdFor.KeyString)
                        ? "\"" + mdFor.KeyString + "\""
                        : mdFor.KeyNumber != null
                            ? "#" + mdFor.KeyNumber
                            : mdFor.KeyGuid != null
                                ? "{" + mdFor.KeyGuid + "}"
                                : "(directly attached)";

                    msg = msg + tr(new[]
                    {
                        (++count).ToString(),
                        md.EntityId.ToString(),
                        md.GetBestTitle(),
                        md.Type.Name,
                        mdFor.TargetType.ToString(),
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
