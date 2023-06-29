using System.Linq;
using ToSic.Eav.Code.Help;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Code.Help
{
    public class ObsoleteHelp
    {
        internal const string IsNotSupportedIn12Plus = "is not supported in Razor12+";

        internal const string IsNotSupportedIn16Plus = "is not supported in Razor16+ / Code16+";

        internal static CodeHelp HelpNotExists12(string property, params string[] replacement) 
            => HelpNotExists(property, IsNotSupportedIn12Plus, replacement?.Select(r => (r, null as string)).ToArray());

        internal static CodeHelp HelpNotExistsPro(string property, params string[] replacement)
            => HelpNotExists(property, IsNotSupportedIn16Plus, replacement?.Select(r => (r, null as string)).ToArray());
        internal static CodeHelp HelpNotExistsPro(string property, params (string Code, string Comment)[] alt)
            => HelpNotExists(property, IsNotSupportedIn16Plus, alt);

        private static CodeHelp HelpNotExists(string property, string notSupported, params (string Code, string Comment)[] alt)
        {
            var first = alt.SafeAny() ? alt[0] : ("unknown", null);
            var better = alt == null || alt.Length == 1
                ? HtmlRec(first)
                : $"<ol>{string.Join("\n", alt.Select(HtmlRec))}</ol>";

            return new CodeHelp(name: $"Object-{property}-DoesNotExist",
                detect: $"error CS0103: The name '{property}' does not exist in the current context",
                uiMessage: $@"
You are calling the '{property}' object which {notSupported}. 
You should probably use '{first.Code}' 
",
                detailsHtml: $@"
You are probably calling <code>{property}</code>. The property <code>{property}</code> {notSupported}. Probably better: 
{better}
"
            );
        }

        private static string HtmlRec((string Code, string Comment) r) 
            => $"<li>{(r.Comment.HasValue() ? r.Comment + " - " : "")}<code>{r.Code}</code></li>";
    }
}
