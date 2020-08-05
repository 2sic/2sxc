using System;
using System.Text.RegularExpressions;

namespace ToSic.Sxc.Web
{
    public abstract class ClientDependencyOptimizer: IClientDependencyOptimizer
    {
        public abstract Tuple<string, bool> Process(string renderedTemplate);

        #region RegEx formulas and static compiled RegEx objects (performance)

        private const string ClientDependencyRegex =
            "\\sdata-enableoptimizations=('|\")(?<Priority>true|[0-9]+)?(?::)?(?<Position>bottom|head|body)?('|\")(>|\\s)";

        private const string ScriptRegExFormula = "<script\\s([^>]*)src=('|\")(?<Src>.*?)('|\")(([^>]*/>)|[^>]*(>.*?</script>))";
        private const string StyleRegExFormula = "<link\\s([^>]*)href=('|\")(?<Src>.*?)('|\")([^>]*)(>.*?</link>|/?>)";
        private const string StyleRelFormula = "('|\"|\\s)rel=('|\")stylesheet('|\")";

        internal static readonly Regex ScriptDetection = new Regex(ScriptRegExFormula, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        internal static readonly Regex StyleDetection = new Regex(StyleRegExFormula, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        internal static readonly Regex StyleRelDetect = new Regex(StyleRelFormula, RegexOptions.IgnoreCase);
        internal static readonly Regex OptimizeDetection = new Regex(ClientDependencyRegex, RegexOptions.IgnoreCase);
        
        #endregion

    }
}