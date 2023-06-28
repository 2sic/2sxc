using System.Linq;
using ToSic.Eav.Code.Help;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Code.Help
{
    public class ObsoleteHelp
    {
        internal const string IsNotSupportedIn12Plus = "is not supported in Razor12+";

        internal const string IsNotSupportedIn16Plus = "is not supported in Razor16+";

        internal static CodeHelp HelpNotExists(string property, params string[] replacement) 
            => HelpNotExists(property, true, replacement);

        internal static CodeHelp HelpNotExists(string property, bool v12, params string[] replacement)
        {
            var firstBetter = !replacement.SafeAny() ? "unknown" : replacement[0];
            var better = !replacement.SafeAny()
                ? $"<code>{firstBetter}</code>"
                : replacement.Length == 1
                    ? $"<code>{firstBetter}</code>"
                    : $"<ol>{string.Join("\n", replacement.Select(r => $"<li><code>{r}</code></li>"))}</ol>";

            var notSupportedText = v12 ? IsNotSupportedIn12Plus : IsNotSupportedIn16Plus;

            return new CodeHelp(name: $"Object-{property}-DoesNotExist",
                detect: $"error CS0103: The name '{property}' does not exist in the current context",
                uiMessage: $@"
You are calling the '{property}' object which {notSupportedText}. 
You should probably use '{firstBetter}' 
",
                detailsHtml: $@"
You are probably calling <code>{property}</code>. The property <code>{property}</code> {notSupportedText}. Probably better: 
{better}
"
            );
        }
    }
}
