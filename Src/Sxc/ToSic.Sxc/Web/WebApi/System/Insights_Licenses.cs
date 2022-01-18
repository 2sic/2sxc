using System.Linq;
using static ToSic.Razor.Blade.Tag;

namespace ToSic.Sxc.Web.WebApi.System
{
    public partial class Insights
    {
        private string Licenses()
        {
            var intro = H1("Licenses and Features")
                      + H2("Licenses")
                      + P("These are the licenses as loaded by the system");

            var rows = Eav.Configuration.Licenses.Licenses.All.Select(l => RowFields(
                EmojiTrueFalse(l.Enabled),
                l.Title,
                l.LicenseKey,
                l.License.Name,
                l.License.Guid,
                EmojiTrueFalse(l.Valid),
                EmojiTrueFalse(l.ValidSignature),
                EmojiTrueFalse(l.ValidFingerprint),
                EmojiTrueFalse(l.ValidVersion),
                EmojiTrueFalse(l.ValidExpired),
                l.Expiration.ToString("yyyy-MM-dd")
            ));

            var msg = intro
                      + Table().Id("table").Wrap(
                          HeadFields(
                              "Enabled", "Title", "License Key on this System", "License Name", "License Guid Identifier", 
                              "Valid", "VSig", "VFP", "VVer", "VExp",
                              "Expires"
                          ),
                          Tbody(rows)
                      );

            msg = msg + Hr() + H2("Features");

            msg = msg + P("Todo...");

            return msg.ToString();
        }
    }
}
