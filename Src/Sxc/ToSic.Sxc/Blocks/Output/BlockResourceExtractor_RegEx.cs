using System.Text.RegularExpressions;
using ToSic.Sxc.Web.ContentSecurityPolicy;

namespace ToSic.Sxc.Blocks.Output
{
    public abstract partial class BlockResourceExtractor
    {
        #region RegEx formulas and static compiled RegEx objects (performance)

        private const string TokenPriority = "Priority";
        private const string TokenPosition = "Position";
        private const string ClientDependencyRegex =
            "\\sdata-enableoptimizations=('|\")(?<Priority>true|[0-9]+)?(?::)?(?<Position>bottom|head|body)?('|\")(>|\\s)";

        // Todo
        private const string CspRegex = "\\s" + CspConstants.CspWhitelistAttribute + "='TODO-CAPTURE-THIS'";

        private const string ScriptSrcFormula = "<script\\s([^>]*)src=('|\")(?<Src>.*?)('|\")(([^>]*/>)|[^>]*(>.*?</script>))";
        private const string ScriptContentFormula = @"<script[^>]*>(?<Content>(.|\n)*?)</script[^>]*>";
        private const string StyleSrcFormula = "<link\\s([^>]*)href=('|\")(?<Src>.*?)('|\")([^>]*)(>.*?</link>|/?>)";
        private const string StyleRelFormula = "('|\"|\\s)rel=('|\")stylesheet('|\")";
        private const string IdFormula = "('|\"|\\s)id=('|\")(?<Id>.*?)('|\")";
        private const string AttributesFormula = "\\s(?<Key>[\\w-]+(?=[^<]*>))=([\"'])(?<Value>.*?[^\\1][\\s\\S]+?)\\1|\\s(?<Key>[\\w-]+(?=.*?))";

        internal static readonly Regex ScriptSrcDetection = new Regex(ScriptSrcFormula, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        internal static readonly Regex ScriptContentDetection = new Regex(ScriptContentFormula, RegexOptions.IgnoreCase | RegexOptions.Multiline);
        internal static readonly Regex StyleDetection = new Regex(StyleSrcFormula, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        internal static readonly Regex StyleRelDetect = new Regex(StyleRelFormula, RegexOptions.IgnoreCase);
        internal static readonly Regex OptimizeDetection = new Regex(ClientDependencyRegex, RegexOptions.IgnoreCase);
        internal static readonly Regex IdDetection = new Regex(IdFormula, RegexOptions.IgnoreCase);
        internal static readonly Regex AttributesDetection = new Regex(AttributesFormula, RegexOptions.IgnoreCase);

        #endregion

        private int GetPriority(Match optMatch, int defValue)
        {
            var priority = (optMatch.Groups[TokenPriority]?.Value ?? "true").ToLowerInvariant();
            var prio = priority == "true" || priority == ""
                ? defValue
                : int.Parse(priority);
            return prio;
        }

    }
}
