using System.Linq;
using static ToSic.Sxc.Plumbing.ParseObject;

namespace ToSic.Sxc.Images
{
    public class ResizeSettingsHelper
    {
        public static MultiResizeRule Find(ResizeSettings resizeSettings, SrcSetType srcSetType, bool useFactors)
        {
            var multiSettings = resizeSettings?.MultiResize;
            if (multiSettings == null) return null;
            var fm = useFactors ? FindSubRule(resizeSettings) : null;

            return srcSetType == SrcSetType.Img
                ? KeepOrUseSubRule(fm, "img") ?? KeepOrUseSubRule(multiSettings.Default, "img")
                : KeepOrUseSubRule(fm, "source") ?? KeepOrUseSubRule(multiSettings.Default, "source");
        }

        private static MultiResizeRule FindSubRule(ResizeSettings resizeSettings)
        {
            var rules = resizeSettings?.MultiResize?.Rules;
            if (rules == null || !rules.Any()) return null;
            var factor = resizeSettings.Factor;
            if (DNearZero(factor)) factor = 1;
            var fm = rules.FirstOrDefault(m => m.Type == MultiResizeRule.RuleForFactor && DNearZero(m.FactorParsed - factor));
            return fm;
        }

        private static MultiResizeRule KeepOrUseSubRule(MultiResizeRule rule, string target)
        {
            if (rule == null) return null;
            if (rule.Sub == null || rule.Sub.Length == 0) return rule;
            return FindRuleForTarget(rule.Sub, target) ?? rule;
        }

        internal static MultiResizeRule FindRuleForTarget(MultiResizeRule[] rules, string target) 
            => rules?.FirstOrDefault(r => r.Type == target);
    }

    public enum SrcSetType
    {
        Img,
        Source
    }

}
