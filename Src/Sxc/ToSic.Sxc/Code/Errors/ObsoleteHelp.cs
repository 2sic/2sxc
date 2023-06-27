using System.Linq;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Code.Errors
{
    public class ObsoleteHelp
    {
        internal const string IsNotSupportedIn12Plus = "is not supported in Razor12+";

        internal static CodeHelp CreateNotExistCodeHelp(string property, params string[] replacement)
        {
            var firstBetter = !replacement.SafeAny() ? "unknown" : replacement[0];
            var better = !replacement.SafeAny()
                ? $"<code>{firstBetter}</code>"
                : replacement.Length == 1
                    ? $"<code>{firstBetter}</code>"
                    : $"<ol>{string.Join("\n", replacement.Select(r => $"<li><code>{r}</code></li>"))}</ol>";

            return new CodeHelp(name: $"Object-{property}-DoesNotExist",
                detect: $"error CS0103: The name '{property}' does not exist in the current context",
                uiMessage: $@"
You are calling the '{property}' object which {IsNotSupportedIn12Plus}. 
You should probably use '{firstBetter}' 
",
                detailsHtml: $@"
You are probably calling <code>{property}</code>. The property <code>{property}</code> {IsNotSupportedIn12Plus}. Probably better: 
{better}
"
            );
        }
    }
}
