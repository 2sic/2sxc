using System.Text.RegularExpressions;
using ToSic.Sxc.Web.PageService;

namespace ToSic.Sxc.Blocks.Output
{
    public abstract partial class BlockResourceExtractor
    {
        #region RegEx formulas and static compiled RegEx objects (performance)

        private const string TokenPriority = "Priority";
        private const string TokenPosition = "Position";

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
