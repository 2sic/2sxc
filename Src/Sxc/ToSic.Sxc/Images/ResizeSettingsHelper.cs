using System.Collections.Generic;
using System.Linq;
using static ToSic.Sxc.Plumbing.ParseObject;

namespace ToSic.Sxc.Images
{
    public class ResizeSettingsHelper
    {
        public static Recipe Find(ResizeSettings resizeSettings, SrcSetType srcSetType, bool useFactors)
        {
            var multiSettings = resizeSettings?.MultiResize;
            if (multiSettings == null) return null;
            var fm = useFactors ? FindSubRule(resizeSettings) : null;

            return srcSetType == SrcSetType.Img
                ? KeepOrUseSubRule(fm, "img") ?? KeepOrUseSubRule(multiSettings.Recipe, "img")
                : KeepOrUseSubRule(fm, "source") ?? KeepOrUseSubRule(multiSettings.Recipe, "source");
        }

        private static Recipe FindSubRule(ResizeSettings resizeSettings)
        {
            var rules = resizeSettings?.MultiResize?.Recipe?.Recipes;
            if (rules == null || !rules.Any()) return null;
            var factor = resizeSettings.Factor;
            if (DNearZero(factor)) factor = 1;
            var fm = rules.FirstOrDefault(m => m.Type == Recipe.RuleForFactor && DNearZero(m.FactorParsed - factor));
            return fm;
        }

        private static Recipe KeepOrUseSubRule(Recipe rule, string target)
        {
            if (rule == null) return null;
            if (rule.Recipes == null || rule.Recipes.Count == 0) return rule;
            return FindRuleForTarget(rule.Recipes, target) ?? rule;
        }

        internal static Recipe FindRuleForTarget(IEnumerable<Recipe> recipes, string target) 
            => recipes?.FirstOrDefault(r => r.Type == target);
    }

    public enum SrcSetType
    {
        Img,
        Source
    }

}
