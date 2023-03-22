//using System.Text.RegularExpressions;
//using ToSic.Sxc.Utils;

//namespace ToSic.Sxc.Blocks.Output
//{
//    public abstract partial class BlockResourceExtractor
//    {
//        #region RegEx formulas and static compiled RegEx objects (performance)

//        //private const string TokenPriority = "Priority";
//        private const string TokenPosition = "Position";

//        #endregion

//        private int GetPriority(Match optMatch, int defaultPriority)
//        {
//            var priority = (optMatch.Groups[RegexUtil.PriorityKey]?.Value ?? "true").ToLowerInvariant();
//            var prio = priority == "true" || priority == ""
//                ? defaultPriority
//                : int.Parse(priority);
//            return prio;
//        }

//    }
//}
