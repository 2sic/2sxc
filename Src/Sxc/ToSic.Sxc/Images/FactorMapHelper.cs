using System.Linq;
using static ToSic.Sxc.Plumbing.ParseObject;

namespace ToSic.Sxc.Images
{
    public class FactorMapHelper
    {
        public static MultiResizeRule Find(ResizeSettings resizeSettings, SrcSetType srcSetType)
        {
            var multiSettings = resizeSettings?.MultiResize;
            if (multiSettings == null) return null;
            var fm = FindSubRule(resizeSettings);

            if (srcSetType == SrcSetType.ImgSrc || srcSetType == SrcSetType.ImgSrcSet)
                return KeepOrUseSubRule(fm, "img") ?? KeepOrUseSubRule(multiSettings.Default, "img");

            return KeepOrUseSubRule(fm, "source") ?? KeepOrUseSubRule(multiSettings.Default, "source");
        }

        private static MultiResizeRule FindSubRule(ResizeSettings resizeSettings)
        {
            var maps = resizeSettings?.MultiResize?.Rules;
            if (maps == null || !maps.Any()) return null;
            var factor = resizeSettings.Factor;
            if (DNearZero(factor)) factor = 1;
            var fm = maps.FirstOrDefault(m => DNearZero(m.FactorParsed - factor));
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
        ImgSrc,
        ImgSrcSet,
        Sources
    }

}
