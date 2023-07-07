using System.Linq;
using ToSic.Eav.Code.Help;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Code.Help
{
    public class ObsoleteHelp
    {
        internal const string IsNotSupportedIn12Plus = "is not supported in Razor12+";

        internal const string IsNotSupportedIn16Plus = "is not supported in RazorPro / CodePro";

        internal static CodeHelp HelpNotExists12(string property, params string[] replacement) 
            => HelpNotExists((property, null), IsNotSupportedIn12Plus, null, replacement?.Select(r => (r, null as string)).ToArray());

        internal static CodeHelp HelpNotExistsPro(string property, params string[] replacement)
            => HelpNotExists((property, null), IsNotSupportedIn16Plus, null, replacement?.Select(r => (r, null as string)).ToArray());

        internal static CodeHelp HelpNotExistsPro(string property, params (string Code, string Comment)[] alt)
            => HelpNotExists((property, null), IsNotSupportedIn16Plus, null, alt);
        internal static CodeHelp HelpNotExistsPro((string Name, string Comments) property, params (string Code, string Comment)[] alt)
            => HelpNotExists(property, IsNotSupportedIn16Plus, null, alt);

        internal static CodeHelp HelpNotExists((string Name, string Comments) property, string notSupported, string linkCode, params (string Code, string Comment)[] alt)
        {
            var first = alt.SafeAny() ? alt[0] : ("unknown", null);
            var better = alt == null || alt.Length == 1
                ? HtmlRec(first)
                : $"<ol>{string.Join("\n", alt.Select(HtmlRec))}</ol>";

            return new CodeHelp(name: $"Object-{property.Name}-DoesNotExist",
                detect: $"error CS0103: The name '{property.Name}' does not exist in the current context",
                linkCode: linkCode,
                uiMessage: $@"
You are calling the '{property.Name}' object which {notSupported}. {property.Comments}
You should probably use '{first.Code}' {first.Comment}
",
                detailsHtml: $@"
You are probably calling <code>{property.Name}</code>.
{(property.Comments.HasValue() ? $"<br><em>{property.Comments}</em><br>": "")}
The property <code>{property.Name}</code> {notSupported}. 
Probably better: 
{better}
"
            );
        }

        internal static string HtmlRec((string Code, string Comment) r)
            => $"<li>{(r.Comment.HasValue() ? r.Comment + " - " : "")}<code>{r.Code}</code></li>";
    }
}
